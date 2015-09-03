using System;
using System.Threading.Tasks;

using CircuitBreaker.Net.Config;

namespace CircuitBreaker.Net.Sample
{
    public class ExternalService
    {
        public void Get()
        {
            throw new Exception();
        }
    }

    public class Program
    {
        public static void Main()
        {
            var externalService = new ExternalService();
            var config = new CircuitBreakerConfig
            {
                TaskScheduler = TaskScheduler.Default
            };

            var circuitBreaker = new CircuitBreaker(config);
            circuitBreaker.Execute(externalService.Get);
        }
    }
}
