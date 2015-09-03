using System;

using CircuitBreaker.Net.Exceptions;

namespace CircuitBreaker.Net.States
{
    public class HalfOpenCircuitBreakerState : ICircuitBreakerState
    {
        private readonly ICircuitBreakerSwitch _switch;
        private readonly ICircuitBreakerInvoker _invoker;
        private readonly TimeSpan _timeout;

        private bool _isEntered;

        public HalfOpenCircuitBreakerState(
            ICircuitBreakerSwitch @switch, 
            ICircuitBreakerInvoker invoker,
            TimeSpan timeout)
        {
            _switch = @switch;
            _invoker = invoker;
            _timeout = timeout;
        }

        public void Enter()
        {
            _isEntered = true;
        }

        public void InvocationFails()
        {
            _switch.OpenCircuit(this);
        }

        public void InvocationSucceeds()
        {
            _switch.CloseCircuit(this);
        }

        public void Invoke(Action action)
        {
            if (_isEntered)
            {
                _isEntered = false;
                _invoker.Invoke(action, _timeout);
            }
            else
            {
                throw new CircuitBreakerOpenException();
            }
        }

        public T Invoke<T>(Func<T> func)
        {
            if (_isEntered)
            {
                _isEntered = false;
                return _invoker.Invoke(func, _timeout);
            }
            else
            {
                throw new CircuitBreakerOpenException();
            }
        }
    }
}