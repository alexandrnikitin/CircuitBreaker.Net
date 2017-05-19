using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace CircuitBreaker.Net.Tests
{
    public class CircuitBreakerEventHandlerTests
    {
        private const int MaxFailures = 1;
        private readonly TimeSpan ResetTimeout = TimeSpan.FromMilliseconds(100);
        private readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(100);
        private readonly CircuitBreaker _sut;
        private readonly ICircuitBreakerEventHandler _eventHandler;

        public CircuitBreakerEventHandlerTests()
        {
            _sut = new CircuitBreaker(TaskScheduler.Default, MaxFailures, Timeout, ResetTimeout);
            _eventHandler = Substitute.For<ICircuitBreakerEventHandler>();
            _sut.EventHandler = _eventHandler;
        }

        private readonly Action _anyAction = () => { };
        private readonly Action _throwAction = () => { throw new Exception(); };

        [Fact]
        public void TriggersEvents()
        {
            try
            {
                _sut.Execute(_throwAction);
            }
            catch (Exception _)
            {
            }
            _eventHandler.Received().OnCircuitOpened(_sut);

            Thread.Sleep(ResetTimeout);
            Thread.Sleep(10);

            _sut.Execute(_anyAction);
            _eventHandler.Received().OnCircuitHalfOpened(_sut);

            _sut.Execute(_anyAction);
            _eventHandler.Received().OnCircuitClosed(_sut);
        }
    }
}