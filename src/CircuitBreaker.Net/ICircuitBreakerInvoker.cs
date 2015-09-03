using System;

namespace CircuitBreaker.Net
{
    public interface ICircuitBreakerInvoker
    {
        void Invoke(Action action);
        void Invoke(Action action, TimeSpan interval);
        T Invoke<T>(Func<T> func);
    }
}