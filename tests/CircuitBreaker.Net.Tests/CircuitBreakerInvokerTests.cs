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
            public void ExepectOpenExceptionWhenActionFails()
            {
                var state = Substitute.For<ICircuitBreakerState>();
                var e = Assert.ThrowsAny<Exception>(() => _sut.InvokeThrough(state, () => { throw new Exception(); }, TimeSpan.FromMilliseconds(100)));
                state.Received().InvocationFails();
                Assert.IsType<Exceptions.CircuitBreakerOpenException>(e);
            }
            [Fact]
            public void ExepectInnerExceptionWhenActionFails()
            {
                var state = Substitute.For<ICircuitBreakerState>();
                var e = Assert.ThrowsAny<Exception>(() => _sut.InvokeThrough(state, () => { throw new ArgumentNullException(); }, TimeSpan.FromMilliseconds(100)));
                state.Received().InvocationFails();
                Assert.IsType<ArgumentNullException>(e.InnerException.InnerException);
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

        public class InvokeThroughAsyncTests : CircuitBreakerInvokerTests
        {
            [Fact]
            public async void ActionFailureInvocation()
            {
                var state = Substitute.For<ICircuitBreakerState>();
                await Assert.ThrowsAnyAsync<Exception>(() => _sut.InvokeThroughAsync(state, () => { throw new Exception(); }, TimeSpan.FromMilliseconds(100)));
                state.Received().InvocationFails();
            }

            [Fact]
            public async void ActionSuccessfulInvocation()
            {
                var state = Substitute.For<ICircuitBreakerState>();
                await _sut.InvokeThroughAsync(state, () => Task.FromResult(false), TimeSpan.FromMilliseconds(100));
                state.Received().InvocationSucceeds();
            }

            [Fact]
            public async void FuncFailureInvocation()
            {
                var state = Substitute.For<ICircuitBreakerState>();
                Func<Task<object>> func = () => { throw new Exception(); };
                await Assert.ThrowsAnyAsync<Exception>(() => _sut.InvokeThroughAsync(state, func, TimeSpan.FromMilliseconds(100)));
                state.Received().InvocationFails();
            }

            [Fact]
            public async void FuncSuccessfulInvocation()
            {
                var state = Substitute.For<ICircuitBreakerState>();
                await _sut.InvokeThroughAsync(state, () => Task.FromResult(new object()), TimeSpan.FromMilliseconds(100));
                state.Received().InvocationSucceeds();
            }
        }
    }
}