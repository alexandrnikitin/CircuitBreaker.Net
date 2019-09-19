using System;

namespace CircuitBreaker.Net.Exceptions
{
    public class CircuitBreakerTimeoutException : CircuitBreakerException
    {
        public CircuitBreakerTimeoutException()
        {
        }
        public CircuitBreakerTimeoutException(Exception inner) : base("CircuitBreaker timed out", inner)
        {

        }
        public CircuitBreakerTimeoutException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}