using System;
using System.Threading;
using System.Threading.Tasks;

using CircuitBreaker.Net.Config;
using CircuitBreaker.Net.Exceptions;

namespace CircuitBreaker.Net
{
    public class CircuitBreaker : ICircuitBreaker
    {
        private readonly TaskScheduler _taskScheduler;
        private readonly TimeSpan _timeout;

        private DateTime _openedDateTime;

        public CircuitBreaker(ICircuitBreakerConfig config)
        {
            _taskScheduler = config.TaskScheduler;
            _timeout = config.Timeout;
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
                HandleOpenCircuit();
                throw new CircuitBreakerOpenException();
            }

            try
            {
                // todo add metrics
                Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
            }
            catch (CircuitBreakerTimeoutException)
            {
                OpenCircuit();
                throw;
            }
            catch (Exception)
            {
                OpenCircuit();
                throw new CircuitBreakerExecutionException();
            }
        }

        public T Execute<T>(Func<T> func)
        {
            if (func == null) throw new ArgumentNullException("func");

            throw new NotImplementedException();
        }

        private void HandleOpenCircuit()
        {
            if (CanCloseCircuit())
            {
                State = CircuitBreakerState.HalfOpen;
            }
        }

        private bool CanCloseCircuit()
        {
            return _openedDateTime + _timeout < DateTime.UtcNow;
        }

        private void OpenCircuit()
        {
            if (State != CircuitBreakerState.Open)
            {
                State = CircuitBreakerState.Open;
                _openedDateTime = DateTime.UtcNow;
            }
        }


    }
}