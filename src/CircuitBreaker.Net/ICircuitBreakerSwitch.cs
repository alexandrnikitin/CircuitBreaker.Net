using CircuitBreaker.Net.States;

namespace CircuitBreaker.Net
{
    public interface ICircuitBreakerSwitch
    {
        void OpenCircuit(ICircuitBreakerState from);
        void AttemptToCloseCircuit(ICircuitBreakerState from);
        void CloseCircuit(ICircuitBreakerState from);
    }
}