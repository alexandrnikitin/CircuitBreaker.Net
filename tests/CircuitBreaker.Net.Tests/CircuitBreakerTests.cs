using System;
using System.Threading;
using System.Threading.Tasks;

using CircuitBreaker.Net.Exceptions;

using Xunit;

namespace CircuitBreaker.Net.Tests
{
    public class CircuitBreakerTests
    {
        private const int MaxFailures = 3;
        private readonly TimeSpan ResetTimeout = TimeSpan.FromMilliseconds(100);
        private readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(100);
        private readonly CircuitBreaker _sut;

        public CircuitBreakerTests()
        {
            _sut = new CircuitBreaker(TaskScheduler.Default, MaxFailures, Timeout, ResetTimeout);
        }

        public class ExecuteActionTests : CircuitBreakerTests
        {
            private readonly Action _anyAction = () => { };
            private readonly Action _throwAction = () => { throw new Exception(); };
            private readonly Action _timeoutAction = () => Thread.Sleep(200);

            [Fact]
            public void SuccessfulExecution()
            {
                for (var i = 0; i < 100; i++)
                {
                    _sut.Execute(_anyAction);
                }
            }

            [Fact]
            public void Timeouts()
            {
                
                Assert.Throws<CircuitBreakerTimeoutException>(() => _sut.Execute(_timeoutAction));
                Assert.Throws<CircuitBreakerTimeoutException>(() => _sut.Execute(_timeoutAction));
                Assert.Throws<CircuitBreakerTimeoutException>(() => _sut.Execute(_timeoutAction));
                Assert.Throws<CircuitBreakerOpenException>(() => _sut.Execute(_anyAction));
            }

            [Fact]
            public void Failures()
            {

                Assert.ThrowsAny<Exception>(() => _sut.Execute(_throwAction));
                Assert.ThrowsAny<Exception>(() => _sut.Execute(_throwAction));
                Assert.ThrowsAny<Exception>(() => _sut.Execute(_throwAction));
                Assert.Throws<CircuitBreakerOpenException>(() => _sut.Execute(_anyAction));
            }

            [Fact]
            public void ResetAfterTimeout()
            {
                Assert.ThrowsAny<Exception>(() => _sut.Execute(_throwAction));
                Assert.ThrowsAny<Exception>(() => _sut.Execute(_throwAction));
                Assert.ThrowsAny<Exception>(() => _sut.Execute(_throwAction));
                
                Thread.Sleep(ResetTimeout);
                Thread.Sleep(10);
                
                _sut.Execute(_anyAction);
            }
        }

        public class ExecuteFuncTests : CircuitBreakerTests
        {
            private readonly Func<object> _anyFunc = () => new object();
            private readonly Func<object> _timeoutFunc = () => { Thread.Sleep(200); return new object(); };
            private readonly Func<object> _throwFunc = () => { throw new Exception(); };

            [Fact]
            public void SuccessfulExecution()
            {
                for (var i = 0; i < 100; i++)
                {
                    _sut.Execute(_anyFunc);
                }
            }

            [Fact]
            public void Timeouts()
            {
                Assert.Throws<CircuitBreakerTimeoutException>(() => _sut.Execute(_timeoutFunc));
                Assert.Throws<CircuitBreakerTimeoutException>(() => _sut.Execute(_timeoutFunc));
                Assert.Throws<CircuitBreakerTimeoutException>(() => _sut.Execute(_timeoutFunc));
                Assert.Throws<CircuitBreakerOpenException>(() => _sut.Execute(_anyFunc));
            }

            [Fact]
            public void Failures()
            {
                Assert.ThrowsAny<Exception>(() => _sut.Execute(_throwFunc));
                Assert.ThrowsAny<Exception>(() => _sut.Execute(_throwFunc));
                Assert.ThrowsAny<Exception>(() => _sut.Execute(_throwFunc));
                Assert.Throws<CircuitBreakerOpenException>(() => _sut.Execute(_anyFunc));
            }

            [Fact]
            public void ResetAfterTimeout()
            {
                Assert.ThrowsAny<Exception>(() => _sut.Execute(_throwFunc));
                Assert.ThrowsAny<Exception>(() => _sut.Execute(_throwFunc));
                Assert.ThrowsAny<Exception>(() => _sut.Execute(_throwFunc));
                
                Thread.Sleep(ResetTimeout);
                Thread.Sleep(100);

                _sut.Execute(_anyFunc);
            }
        }
    }
}