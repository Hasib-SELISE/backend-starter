using Application.Common.Abstractions;
using Polly;
using Polly.Extensions.Http;

namespace Application.Common.Policies;

public static class HttpPolicy
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(IRmwSettings rmwSettings)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(int.Parse(rmwSettings.PollySettings.MaxRetry), retryAttempt =>
            {
                var timeSpan = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                Console.WriteLine($"Waiting for {timeSpan} seconds");
                return timeSpan;
            });
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(IRmwSettings rmwSettings)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(int.Parse(rmwSettings.PollySettings.AllowedErrorCountBeforeCircuitBreaks), TimeSpan.FromSeconds(5));
    }
}