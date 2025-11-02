// csharp

using simple_api.adapter;

namespace simple_api.Config;


public class MdcMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MdcMiddleware> _logger;

    public MdcMiddleware(RequestDelegate next, ILogger<MdcMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Exemplo: popula MDC com header X-Request-Id e mantém via scope
        if (context.Request.Headers.TryGetValue("X-Request-Id", out var reqId))
        {
            using (Mdc.BeginScope("X-Request-Id", reqId.ToString()))
            {
                _logger.LogInformation("MDC X-Request-Id = {Id}", Mdc.Get("X-Request-Id"));
                await _next(context);
            }
            return;
        }

        await _next(context);
    }
}
