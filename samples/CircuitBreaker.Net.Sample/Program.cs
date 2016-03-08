using System;
using System.Threading;
using System.Threading.Tasks;

using CircuitBreaker.Net.Exceptions;

namespace CircuitBreaker.Net.Sample
{
    public class Program
    {
        public static void Main()
        {
            var externalService = new ExternalService();

            var circuitBreaker = new CircuitBreaker(
                TaskScheduler.Default,
                maxFailures: 2,
                invocationTimeout: TimeSpan.FromMilliseconds(10),
                circuitResetTimeout: TimeSpan.FromMilliseconds(1000));

            TryExecute(circuitBreaker, externalService.Get);
            TryExecute(circuitBreaker, () => Thread.Sleep(100));
            TryExecute(circuitBreaker, externalService.Get);

            TryExecuteAsync(circuitBreaker, externalService.GetAsync).Wait();
            TryExecuteAsync(circuitBreaker, () => Task.Delay(100)).Wait();
            TryExecuteAsync(circuitBreaker, externalService.GetAsync).Wait();

        }

        private static void TryExecute(ICircuitBreaker circuitBreaker, Action action)
        {
            try
            {
                circuitBreaker.Execute(action);
            }
            catch (CircuitBreakerOpenException)
            {
                Console.WriteLine("CircuitBreakerOpenException");
            }
            catch (CircuitBreakerTimeoutException)
            {
                Console.WriteLine("CircuitBreakerTimeoutException");
            }
            catch (Exception)
            {
                Console.WriteLine("Exception");
            }
        }

        private static async Task TryExecuteAsync(ICircuitBreaker circuitBreaker, Func<Task> action)
        {
            try
            {
                await circuitBreaker.ExecuteAsync(action);
            }
            catch (CircuitBreakerOpenException)
            {
                Console.WriteLine("CircuitBreakerOpenException");
            }
            catch (CircuitBreakerTimeoutException)
            {
                Console.WriteLine("CircuitBreakerTimeoutException");
            }
            catch (Exception)
            {
                Console.WriteLine("Exception");
            }
        }
    }
}
