using System;
using System.Threading;
using System.Threading.Tasks;

using CircuitBreaker.Net.Exceptions;

namespace CircuitBreaker.Net
{
    public class CircuitBreaker : ICircuitBreaker
    {
        private readonly TaskScheduler _taskScheduler;

        public CircuitBreaker(TaskScheduler taskScheduler)
        {
            _taskScheduler = taskScheduler;
        }

        public string Id { get; private set; }

        public CircuitBreakerState State { get; private set; }

        public void Dispose()
        {
        }

        public void Execute(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");

            if (State == CircuitBreakerState.Open)
            {
                // todo try to close
                throw new CircuitBreakerOpenException();
            }

            try
            {
                // todo add metrics
                Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
            }
            catch (CircuitBreakerTimeoutException)
            {
                // todo open circuit
                throw;
            }
            catch (Exception)
            {
                // todo open
                throw new CircuitBreakerExecutionException();
            }
        }

        public T Execute<T>(Func<T> func)
        {
            if (func == null) throw new ArgumentNullException("func");

            throw new NotImplementedException();
        }
    }
}