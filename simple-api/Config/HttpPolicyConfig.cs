// csharp
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Wrap;

namespace simple_api.Config
{
    public static class HttpPolicyConfig
    {
        public static IAsyncPolicy<HttpResponseMessage> GetPolicy(ILogger? logger = null)
        {
            // Retry: 3 tentativas com backoff exponencial para erros transitórios
            var retry = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, ctx) =>
                    {
                        var reason = outcome.Exception?.Message ?? outcome?.Result?.StatusCode.ToString() ?? "Unknown";
                        logger?.LogWarning("Retry {Retry} after {Delay} due to {Reason}", retryCount, timespan, reason);
                    });

            // Circuit Breaker: abre após 5 falhas consecutivas por 30s
            var circuitBreaker = HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (outcome, breakDelay) =>
                    {
                        var reason = outcome.Exception?.Message ?? outcome?.Result?.StatusCode.ToString() ?? "Unknown";
                        logger?.LogWarning("Circuit broken for {Delay} due to {Reason}", breakDelay, reason);
                    },
                    onReset: () => logger?.LogInformation("Circuit reset"),
                    onHalfOpen: () => logger?.LogInformation("Circuit half-open"));

            // Fallback: retorna uma resposta de fallback quando tudo falhar
            var fallbackResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{ \"message\": \"fallback\" }"),
                ReasonPhrase = "Fallback"
            };

            var fallback = Policy<HttpResponseMessage>
                .Handle<Exception>()
                .OrResult(r => !r.IsSuccessStatusCode)
                .FallbackAsync(fallbackResponse, onFallbackAsync: (outcome, context) =>
                {
                    var reason = outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString() ?? "Unknown";
                    logger?.LogWarning("Fallback executed due to {Reason}", reason);
                    return Task.CompletedTask;
                });

            // Combine: fallback wraps retry que por sua vez envolve o circuit breaker
            return Policy.WrapAsync(fallback, retry, circuitBreaker);
        }
    }
}
