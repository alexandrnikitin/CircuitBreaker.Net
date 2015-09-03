using System;
using System.Threading.Tasks;

namespace CircuitBreaker.Net.Config
{
    public interface ICircuitBreakerConfig
    {
        TaskScheduler TaskScheduler { get; set; }
        TimeSpan Timeout { get; set; }
        int MaxFailures { get; set; }
    }
}