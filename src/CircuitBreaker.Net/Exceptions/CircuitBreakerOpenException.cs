using System;

namespace CircuitBreaker.Net.Exceptions
{
    public class CircuitBreakerOpenException : CircuitBreakerException
    {
        public CircuitBreakerOpenException()
        {
        }
        public CircuitBreakerOpenException(Exception inner) : base("Openning CircuitBreaker failed", inner)
        {

        }
        public CircuitBreakerOpenException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}