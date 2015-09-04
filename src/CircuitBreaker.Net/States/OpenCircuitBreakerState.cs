using System;

using CircuitBreaker.Net.Exceptions;

namespace CircuitBreaker.Net.States
{
    internal class OpenCircuitBreakerState : ICircuitBreakerState
    {
        private readonly ICircuitBreakerInvoker _invoker;
        private readonly TimeSpan _resetTimeSpan;
        private readonly ICircuitBreakerSwitch _switch;

        public OpenCircuitBreakerState(
            ICircuitBreakerSwitch @switch, 
            ICircuitBreakerInvoker invoker, 
            TimeSpan resetTimeSpan)
        {
            _switch = @switch;
            _invoker = invoker;
            _resetTimeSpan = resetTimeSpan;
        }

        public void Enter()
        {
            _invoker.InvokeScheduled(() => _switch.AttemptToCloseCircuit(this), _resetTimeSpan);
        }

        public void InvocationFails()
        {
        }

        public void InvocationSucceeds()
        {
        }

        public void Invoke(Action action)
        {
            throw new CircuitBreakerOpenException();
        }

        public T Invoke<T>(Func<T> func)
        {
            throw new CircuitBreakerOpenException();
        }
    }
}