using System;
using System.Threading;
using System.Threading.Tasks;

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

            // todo add metrics
            Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
        }

        public T Execute<T>(Func<T> func)
        {
            throw new NotImplementedException();
        }
    }
}