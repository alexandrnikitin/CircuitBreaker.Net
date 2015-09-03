using System.Threading.Tasks;

namespace CircuitBreaker.Net.Config
{
    public interface ICircuitBreakerConfig
    {
        TaskScheduler TaskScheduler { get; set; }
    }
}