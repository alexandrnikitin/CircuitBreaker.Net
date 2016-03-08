using System;
using System.Threading;
using System.Threading.Tasks;

using CircuitBreaker.Net.Exceptions;

namespace CircuitBreaker.Net.States
{
    internal class HalfOpenCircuitBreakerState : ICircuitBreakerState
    {
        private const int Invoking = 1;
        private const int NotInvoking = 0;

        private readonly ICircuitBreakerSwitch _switch;
        private readonly ICircuitBreakerInvoker _invoker;
        private readonly TimeSpan _timeout;

        private int _isBeingInvoked;

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
            _isBeingInvoked = NotInvoking;
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
            if (Interlocked.CompareExchange(ref _isBeingInvoked, Invoking, NotInvoking) == NotInvoking)
            {
                _invoker.InvokeThrough(this, action, _timeout);
            }
            else
            {
                throw new CircuitBreakerOpenException();
            }
        }

        public T Invoke<T>(Func<T> func)
        {
            if (Interlocked.CompareExchange(ref _isBeingInvoked, Invoking, NotInvoking) == NotInvoking)
            {
                return _invoker.InvokeThrough(this, func, _timeout);
            }
            else
            {
                throw new CircuitBreakerOpenException();
            }
        }

        public async Task InvokeAsync(Func<Task> func)
        {
            if (Interlocked.CompareExchange(ref _isBeingInvoked, Invoking, NotInvoking) == NotInvoking)
            {
                await _invoker.InvokeThroughAsync(this, func, _timeout);
            }
            else
            {
                throw new CircuitBreakerOpenException();
            }
;
        }

        public async Task<T> InvokeAsync<T>(Func<Task<T>> func)
        {
            if (Interlocked.CompareExchange(ref _isBeingInvoked, Invoking, NotInvoking) == NotInvoking)
            {
                return await _invoker.InvokeThroughAsync(this, func, _timeout);
            }
            else
            {
                throw new CircuitBreakerOpenException();
            }
        }
    }
}