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

    class Program
    {
        static void Main(string[] args)
        {
            var externalService = new ExternalService();
            var circuitBreaker = new CircuitBreaker(TaskScheduler.Default);
            circuitBreaker.Execute(externalService.Get);

        }
    }
}
