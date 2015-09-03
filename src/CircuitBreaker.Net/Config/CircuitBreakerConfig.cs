using System;
using System.Threading.Tasks;

namespace CircuitBreaker.Net.Config
{
    public class CircuitBreakerConfig : ICircuitBreakerConfig
    {
        public ICircuitBreakerInvoker CircuitBreakerInvoker { get; set; }
        public ICircuitBreakerSwitch CircuitBreakerSwitch { get; set; }
        public TimeSpan CircuitResetTimeout { get; set; }
        public TimeSpan InvocationTimeout { get; set; }
        public int MaxFailures { get; set; }
        public TaskScheduler TaskScheduler { get; set; }
    }
}