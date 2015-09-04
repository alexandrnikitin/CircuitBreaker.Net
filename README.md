# CircuitBreaker.Net
Circuit Breaker pattern implementation for .NET. More info about the pattern [on MSDN.](https://msdn.microsoft.com/en-us/library/dn589784.aspx)

## Install

It's available via [nuget package](https://www.nuget.org/packages/CircuitBreaker.Net)  
PM> `Install-Package CircuitBreaker.Net`

Or nuget package [with sources only](https://www.nuget.org/packages/CircuitBreaker.Net.Source)  
PM> `Install-Package CircuitBreaker.Net.Source`

## Example Usage

```csharp
// Initialize the circuit breaker
var circuitBreaker = new CircuitBreaker(
    TaskScheduler.Default,
    maxFailures: 3,
    invocationTimeout: TimeSpan.FromMilliseconds(100),
    circuitResetTimeout: TimeSpan.FromMilliseconds(10000));
    
try
{
    // perform a potentially fragile call through the circuit breaker
    circuitBreaker.Execute(externalService.Call);
}
catch (CircuitBreakerOpenException)
{
    // the service is unavailable, failover here
}
catch (CircuitBreakerTimeoutException)
{
    // handle timeouts
}
catch (Exception)
{
    // handle other unexpected exceptions
}

```
