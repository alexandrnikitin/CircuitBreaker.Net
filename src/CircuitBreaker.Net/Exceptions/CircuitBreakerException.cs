using System;

namespace CircuitBreaker.Net.Exceptions
{
    public abstract class CircuitBreakerException : Exception
    {
        public CircuitBreakerException()
        {
        }
        public CircuitBreakerException(string message, Exception inner): base(message, inner)
        {
        }
    }
}