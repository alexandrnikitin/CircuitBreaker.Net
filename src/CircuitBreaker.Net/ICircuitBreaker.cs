using System;

namespace CircuitBreaker.Net
{
    public interface ICircuitBreaker
    {
        void Execute(Action action);
        T Execute<T>(Func<T> func);
    }
}