using System;

namespace CircuitBreaker.Net
{
    public interface ICircuitBreakerEvents
    {
        event EventHandler Closed;
        event EventHandler Opened;
        event EventHandler HalfOpened;
    }
}