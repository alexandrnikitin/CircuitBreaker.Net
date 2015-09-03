using System.Threading.Tasks;

namespace CircuitBreaker.Net.Config
{
    public class CircuitBreakerConfig : ICircuitBreakerConfig
    {
        public TaskScheduler TaskScheduler { get; set; }
    }
}