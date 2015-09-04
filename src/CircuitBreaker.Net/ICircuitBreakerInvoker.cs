using System;

using CircuitBreaker.Net.States;

namespace CircuitBreaker.Net
{
    public interface ICircuitBreakerInvoker
    {
        void InvokeScheduled(Action action, TimeSpan interval);
        void InvokeThrough(ICircuitBreakerState state, Action action, TimeSpan timeout);
        T InvokeThrough<T>(ICircuitBreakerState state, Func<T> func, TimeSpan timeout);
    }
}