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
    }

    public class Program
    {
        public static void Main()
        {
            var externalService = new ExternalService();

            var circuitBreaker = new CircuitBreaker(
                TaskScheduler.Default,
                maxFailures: 3,
                invocationTimeout: TimeSpan.FromMilliseconds(10),
                circuitResetTimeout: TimeSpan.FromMilliseconds(10));

            circuitBreaker.Execute(externalService.Get);
        }
    }
}
