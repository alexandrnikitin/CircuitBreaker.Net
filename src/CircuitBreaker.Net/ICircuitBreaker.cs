using System;
using System.Threading.Tasks;

namespace CircuitBreaker.Net
{
    public interface ICircuitBreaker
    {
        void Execute(Action action);
        T Execute<T>(Func<T> func);
        Task ExecuteAsync(Func<Task> func);
        Task<T> ExecuteAsync<T>(Func<Task<T>> func);
    }
}