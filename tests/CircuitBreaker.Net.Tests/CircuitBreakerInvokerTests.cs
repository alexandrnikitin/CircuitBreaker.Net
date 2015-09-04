using System;
using System.Threading;
using System.Threading.Tasks;

using CircuitBreaker.Net.States;

using NSubstitute;

using Xunit;

namespace CircuitBreaker.Net.Tests
{
    public class CircuitBreakerInvokerTests
    {
        private readonly CircuitBreakerInvoker _sut;

        public CircuitBreakerInvokerTests()
        {
            _sut = new CircuitBreakerInvoker(TaskScheduler.Default);
        }

        public class InvokeScheduledTests : CircuitBreakerInvokerTests
        {
            [Fact]
            public void ScheduledActionActionSucceessfulInvocation()
            {
                var isExecuted = false;
                var reset = new ManualResetEventSlim(false);

                _sut.InvokeScheduled(() => { isExecuted = true; reset.Set(); }, new TimeSpan());
                
                reset.Wait(100);
                Assert.True(isExecuted);
            }
        }

        public class InvokeThroughTests : CircuitBreakerInvokerTests
        {
            [Fact]
            public void ActionFailureInvocation()
            {
                var state = Substitute.For<ICircuitBreakerState>();
                Assert.ThrowsAny<Exception>(() => _sut.InvokeThrough(state, () => { throw new Exception(); }, TimeSpan.FromMilliseconds(100)));
                state.Received().InvocationFails();
            }

            [Fact]
            public void ActionSucceessfulInvocation()
            {
                var state = Substitute.For<ICircuitBreakerState>();
                _sut.InvokeThrough(state, () => { }, TimeSpan.FromMilliseconds(100));
                state.Received().InvocationSucceeds();
            }

            [Fact]
            public void FuncFailureInvocation()
            {
                var state = Substitute.For<ICircuitBreakerState>();
                Func<object> func = () => { throw new Exception(); };
                Assert.ThrowsAny<Exception>(() => _sut.InvokeThrough(state, func, TimeSpan.FromMilliseconds(100)));
                state.Received().InvocationFails();
            }

            [Fact]
            public void FuncSucceessfulInvocation()
            {
                var state = Substitute.For<ICircuitBreakerState>();
                _sut.InvokeThrough(state, () => new object(), TimeSpan.FromMilliseconds(100));
                state.Received().InvocationSucceeds();
            }
        }
    }
}