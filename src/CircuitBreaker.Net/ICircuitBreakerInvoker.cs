using System;
using System.Threading.Tasks;

using CircuitBreaker.Net.States;

namespace CircuitBreaker.Net
{
    internal interface ICircuitBreakerInvoker
    {
        void InvokeScheduled(Action action, TimeSpan interval);
        void InvokeThrough(ICircuitBreakerState state, Action action, TimeSpan timeout);
        T InvokeThrough<T>(ICircuitBreakerState state, Func<T> func, TimeSpan timeout);
        Task InvokeThroughAsync(ICircuitBreakerState state, Func<Task> func, TimeSpan timeout);
        Task<T> InvokeThroughAsync<T>(ICircuitBreakerState state, Func<Task<T>> func, TimeSpan timeout);
    }
}