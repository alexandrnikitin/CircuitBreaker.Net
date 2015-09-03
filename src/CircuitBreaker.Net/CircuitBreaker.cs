using System;

using CircuitBreaker.Net.Config;
using CircuitBreaker.Net.States;

namespace CircuitBreaker.Net
{
    public class CircuitBreaker : ICircuitBreaker, ICircuitBreakerSwitch
    {
        private readonly ICircuitBreakerState _closeState;
        private readonly ICircuitBreakerState _halfOpenState;
        private readonly ICircuitBreakerState _openState;
        private ICircuitBreakerState _currentState;

        public CircuitBreaker(ICircuitBreakerConfig config)
        {
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
            Trip(from, _halfOpenState);
        }

        void ICircuitBreakerSwitch.CloseCircuit(ICircuitBreakerState @from)
        {
            Trip(from, _closeState);
        }

        void ICircuitBreakerSwitch.OpenCircuit(ICircuitBreakerState @from)
        {
            Trip(from, _openState);
        }

        private void Trip(ICircuitBreakerState from, ICircuitBreakerState to)
        {
            _currentState = to;
            to.Enter();
        }
    }
}