using simple_api.adapter;

namespace simple_api.Config;

public class AppInterceptor {
    
    private readonly RequestDelegate _next;
    private readonly ILogger<AppInterceptor> _logger;
    
    public AppInterceptor(RequestDelegate next, ILogger<AppInterceptor> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);

        var headers = context.Request.Headers;
        _logger.LogInformation("headers {Headers}", headers);
        _logger.LogInformation("Header x: {}", headers["test"]);
        foreach (var header in context.Request.Headers)
        {
            if (header.Key == "test") {
                Mdc.Put("agencia", header.Value);
            }
        }
        
        await _next(context);
        
        _logger.LogInformation("Finished handling request.");
    }
    
}