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

        public ICircuitBreakerEventHandler EventHandler { get; set; }

        public virtual void Execute(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");

            _currentState.Invoke(action);
        }

        public virtual T Execute<T>(Func<T> func)
        {
            if (func == null) throw new ArgumentNullException("func");
            
            return _currentState.Invoke(func);
        }

        public virtual async Task ExecuteAsync(Func<Task> func)
        {
            if (func == null) throw new ArgumentNullException("func");

            await _currentState.InvokeAsync(func);
        }

        public virtual async Task<T> ExecuteAsync<T>(Func<Task<T>> func)
        {
            if (func == null) throw new ArgumentNullException("func");

            return await _currentState.InvokeAsync(func);
        }

        void ICircuitBreakerSwitch.AttemptToCloseCircuit(ICircuitBreakerState from)
        {
            var tripped = TryToTrip(from, _halfOpenedState);
            if (tripped) EventHandler?.OnCircuitHalfOpened(this);
        }

        void ICircuitBreakerSwitch.CloseCircuit(ICircuitBreakerState from)
        {
            var tripped = TryToTrip(from, _closedState);
            if (tripped) EventHandler?.OnCircuitClosed(this);
        }

        void ICircuitBreakerSwitch.OpenCircuit(ICircuitBreakerState from)
        {
            var tripped = TryToTrip(from, _openedState);
            if (tripped) EventHandler?.OnCircuitOpened(this);
        }

        private bool TryToTrip(ICircuitBreakerState from, ICircuitBreakerState to)
        {
            if (Interlocked.CompareExchange(ref _currentState, to, from) == from)
            {
                to.Enter();
                return true;
            }
            return false;
        }
    }
}