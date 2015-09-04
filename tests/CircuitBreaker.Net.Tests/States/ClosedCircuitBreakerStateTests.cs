using System;

using CircuitBreaker.Net.States;

using NSubstitute;

using Xunit;

namespace CircuitBreaker.Net.Tests.States
{
    public class ClosedCircuitBreakerStateTests
    {
        private const int MaxFailures = 3;

        private readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(100);
        private readonly ICircuitBreakerInvoker _invoker;
        private readonly ClosedCircuitBreakerState _sut;
        private readonly ICircuitBreakerSwitch _switch;

        public ClosedCircuitBreakerStateTests()
        {
            _switch = Substitute.For<ICircuitBreakerSwitch>();
            _invoker = Substitute.For<ICircuitBreakerInvoker>();
            _sut = new ClosedCircuitBreakerState(_switch, _invoker, MaxFailures, Timeout);
        }

        public class InvocationFailsTests : ClosedCircuitBreakerStateTests
        {
            [Fact]
            public void OpensCircuitAfterMaxFailures()
            {
                _sut.InvocationFails();
                _switch.DidNotReceive().OpenCircuit(Arg.Any<ICircuitBreakerState>());
                _sut.InvocationFails();
                _sut.InvocationFails();
                _switch.Received().OpenCircuit(Arg.Is(_sut));
            }
        }
    }
}