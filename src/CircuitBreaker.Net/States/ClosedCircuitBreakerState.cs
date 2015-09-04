using System;
using System.Threading;

namespace CircuitBreaker.Net.States
{
    public class ClosedCircuitBreakerState : ICircuitBreakerState
    {
        private readonly ICircuitBreakerInvoker _invoker;
        private readonly int _maxFailures;
        private readonly TimeSpan _timeout;
        private readonly ICircuitBreakerSwitch _switch;

        private int _failures;

        public ClosedCircuitBreakerState(
            ICircuitBreakerSwitch @switch, 
            ICircuitBreakerInvoker invoker, 
            int maxFailures,
            TimeSpan timeout)
        {
            _maxFailures = maxFailures;
            _timeout = timeout;
            _switch = @switch;
            _invoker = invoker;
        }

        public void Enter()
        {
            _failures = 0;
        }

        public void InvocationFails()
        {
            if (Interlocked.Increment(ref _failures) == _maxFailures)
            {
                _switch.OpenCircuit(this);
            }
        }

        public void InvocationSucceeds()
        {
            _failures = 0;
        }

        public void Invoke(Action action)
        {
            _invoker.Invoke(action, _timeout);
        }

        public T Invoke<T>(Func<T> func)
        {
            return _invoker.Invoke(func, _timeout);
        }
    }
}