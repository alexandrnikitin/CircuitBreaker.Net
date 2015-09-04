using System;

namespace CircuitBreaker.Net.States
{
    internal interface ICircuitBreakerState
    {
        void Enter();
        void InvocationFails();
        void InvocationSucceeds();
        void Invoke(Action action);
        T Invoke<T>(Func<T> func);
    }
}