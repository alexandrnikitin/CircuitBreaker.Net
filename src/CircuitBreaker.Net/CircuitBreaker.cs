using System;
using System.Threading;
using System.Threading.Tasks;

using CircuitBreaker.Net.States;

namespace CircuitBreaker.Net
{
    public class CircuitBreaker : ICircuitBreaker, ICircuitBreakerSwitch
    {
        private readonly ICircuitBreakerState _closedState;
        private readonly ICircuitBreakerState _halfOpenedState;
        private readonly ICircuitBreakerState _openedState;
        
        private ICircuitBreakerState _currentState;

        public CircuitBreaker(
            TaskScheduler taskScheduler,
            int maxFailures,
            TimeSpan invocationTimeout,
            TimeSpan circuitResetTimeout)
        {
            var invoker = new CircuitBreakerInvoker(taskScheduler);

            _closedState = new ClosedCircuitBreakerState(
                this,
                invoker,
                maxFailures,
                invocationTimeout);

            _halfOpenedState = new HalfOpenCircuitBreakerState(
                this,
                invoker,
                invocationTimeout);

            _openedState = new OpenCircuitBreakerState(
                this,
                invoker, 
                circuitResetTimeout);

            _currentState = _closedState;
        }

        public void Execute(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");

            try
            {
                _currentState.Invoke(action);
            }
            catch (Exception)
            {
                _currentState.InvocationFails();
                throw;
            }

            _currentState.InvocationSucceeds();
        }

        public T Execute<T>(Func<T> func)
        {
            if (func == null) throw new ArgumentNullException("func");

            T result;
            try
            {
                result = _currentState.Invoke(func);
            }
            catch (Exception)
            {
                _currentState.InvocationFails();
                throw;
            }

            _currentState.InvocationSucceeds();
            return result;
        }

        void ICircuitBreakerSwitch.AttemptToCloseCircuit(ICircuitBreakerState @from)
        {
            Trip(from, _halfOpenedState);
        }

        void ICircuitBreakerSwitch.CloseCircuit(ICircuitBreakerState @from)
        {
            Trip(from, _closedState);
        }

        void ICircuitBreakerSwitch.OpenCircuit(ICircuitBreakerState @from)
        {
            Trip(from, _openedState);
        }

        private void Trip(ICircuitBreakerState from, ICircuitBreakerState to)
        {
            if (Interlocked.CompareExchange(ref _currentState, to, from) == from)
            {
                to.Enter();
            }
        }
    }
}