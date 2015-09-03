using System;

using CircuitBreaker.Net.Config;
using CircuitBreaker.Net.States;

namespace CircuitBreaker.Net
{
    public class CircuitBreaker : ICircuitBreaker, ICircuitBreakerSwitch
    {
        private readonly ICircuitBreakerState _closedState;
        private readonly ICircuitBreakerState _halfOpenedState;
        private readonly ICircuitBreakerState _openedState;
        
        private ICircuitBreakerState _currentState;

        public CircuitBreaker(ICircuitBreakerConfig config)
        {
            _closedState = new ClosedCircuitBreakerState(
                config.CircuitBreakerSwitch, 
                config.CircuitBreakerInvoker, 
                config.MaxFailures, 
                config.InvocationTimeout);

            _halfOpenedState = new HalfOpenCircuitBreakerState(
                config.CircuitBreakerSwitch, 
                config.CircuitBreakerInvoker, 
                config.InvocationTimeout);

            _openedState = new OpenCircuitBreakerState(
                config.CircuitBreakerSwitch, 
                config.CircuitBreakerInvoker, 
                config.CircuitResetTimeout);

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
            _currentState = to;
            to.Enter();
        }
    }
}