using System;
using System.Threading.Tasks;

namespace CircuitBreaker.Net.Config
{
    public interface ICircuitBreakerConfig
    {
        ICircuitBreakerInvoker CircuitBreakerInvoker { get; set; }
        ICircuitBreakerSwitch CircuitBreakerSwitch { get; set; }
        TimeSpan CircuitResetTimeout { get; set; }
        TimeSpan InvocationTimeout { get; set; }
        int MaxFailures { get; set; }
        TaskScheduler TaskScheduler { get; set; }
    }
}