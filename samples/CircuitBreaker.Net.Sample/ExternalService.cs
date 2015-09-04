using System;

namespace CircuitBreaker.Net.Sample
{
    public class ExternalService
    {
        public void Get()
        {
            throw new Exception();
        }
    }
}