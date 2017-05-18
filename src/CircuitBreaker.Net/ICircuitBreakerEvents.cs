using System;

namespace CircuitBreaker.Net
{
    public interface ICircuitBreakerEvents
    {
        void CircuitClosed(ICircuitBreaker circuitBreaker);
        void CircuitOpened(ICircuitBreaker circuitBreaker);
        void CircuitHalfOpened(ICircuitBreaker circuitBreaker);
    }
}