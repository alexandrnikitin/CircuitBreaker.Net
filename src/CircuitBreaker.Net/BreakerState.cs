namespace CircuitBreaker.Net
{
    public enum CircuitBreakerState
    {
        None = 0,
        Closed, 
        Open, 
        HalfOpen
    }
}