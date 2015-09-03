using System;

namespace CircuitBreaker.Net
{
    public interface ICircuitBreaker : IDisposable
    {
        string Id { get; }
        CircuitBreakerState State { get; }
        void Execute(Action action);
        T Execute<T>(Func<T> func);
    }
}