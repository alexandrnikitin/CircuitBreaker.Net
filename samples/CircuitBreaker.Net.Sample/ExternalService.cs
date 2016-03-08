using System;
using System.Threading.Tasks;

namespace CircuitBreaker.Net.Sample
{
    public class ExternalService
    {
        public void Get()
        {
            throw new Exception();
        }

        public async Task GetAsync()
        {
            throw new Exception();
        }
    }
}