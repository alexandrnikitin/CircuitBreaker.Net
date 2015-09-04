using System;
using System.Threading;

using CircuitBreaker.Net.Exceptions;

namespace CircuitBreaker.Net.States
{
    public class HalfOpenCircuitBreakerState : ICircuitBreakerState
    {
        private const int Entered = 1;
        private const int NotEntered = 0;

        private readonly ICircuitBreakerSwitch _switch;
        private readonly ICircuitBreakerInvoker _invoker;
        private readonly TimeSpan _timeout;

        private int _isEntered;

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
            _isEntered = Entered;
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
            if (Interlocked.CompareExchange(ref _isEntered, NotEntered, Entered) == Entered)
            {
                _invoker.Invoke(action, _timeout);
            }
            else
            {
                throw new CircuitBreakerOpenException();
            }
        }

        public T Invoke<T>(Func<T> func)
        {
            if (Interlocked.CompareExchange(ref _isEntered, NotEntered, Entered) == Entered)
            {
                return _invoker.Invoke(func, _timeout);
            }
            else
            {
                throw new CircuitBreakerOpenException();
            }
        }
    }
}