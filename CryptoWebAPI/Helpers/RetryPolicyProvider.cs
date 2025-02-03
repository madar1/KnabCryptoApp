using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace CryptoWebAPI.Helpers;
public static class RetryPolicyProvider
{
    public static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return Policy
            .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests || !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff: 2s, 4s, 8s
                (result, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount} after {timeSpan.Seconds} seconds due to {result.Result.StatusCode}");
                });
    }

    public static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1),
                (result, duration) => Console.WriteLine($"Circuit breaker opened for {duration.TotalSeconds} seconds"),
                () => Console.WriteLine("Circuit breaker reset."));
    }
}
