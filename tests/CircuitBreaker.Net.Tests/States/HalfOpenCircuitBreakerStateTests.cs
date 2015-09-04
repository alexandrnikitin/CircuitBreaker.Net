using System;

using CircuitBreaker.Net.Exceptions;
using CircuitBreaker.Net.States;

using NSubstitute;

using Xunit;

namespace CircuitBreaker.Net.Tests.States
{
    public class HalfOpenCircuitBreakerStateTests
    {
        private readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(100);
        private readonly ICircuitBreakerInvoker _invoker;
        private readonly HalfOpenCircuitBreakerState _sut;
        private readonly ICircuitBreakerSwitch _switch;

        public HalfOpenCircuitBreakerStateTests()
        {
            _switch = Substitute.For<ICircuitBreakerSwitch>();
            _invoker = Substitute.For<ICircuitBreakerInvoker>();
            _sut = new HalfOpenCircuitBreakerState(_switch, _invoker, Timeout);
        }

        public class InvokeTests : HalfOpenCircuitBreakerStateTests
        {
            [Fact]
            public void InvokesOnlyFirstAction()
            {
                Action action = () => {};
                _sut.Invoke(action);
                _invoker.Received().InvokeThrough(Arg.Is(_sut), action, Timeout);
                _invoker.ClearReceivedCalls();

                Assert.Throws<CircuitBreakerOpenException>(() => _sut.Invoke(action));
                _invoker.DidNotReceive().InvokeThrough(Arg.Any<ICircuitBreakerState>(), Arg.Any<Action>(), Arg.Any<TimeSpan>());
            }

            [Fact]
            public void InvokesOnlyFirstFunc()
            {
                Func<object> func = () => new object();
                _sut.Invoke(func);
                _invoker.Received().InvokeThrough(Arg.Is(_sut), func, Timeout);
                _invoker.ClearReceivedCalls();

                Assert.Throws<CircuitBreakerOpenException>(() => _sut.Invoke(func));
                _invoker.DidNotReceive().InvokeThrough(Arg.Any<ICircuitBreakerState>(), Arg.Any<Action>(), Arg.Any<TimeSpan>());
            }
        }
    }
}