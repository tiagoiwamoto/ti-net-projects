namespace simple_api.Config;

public static class MiddlewareRegister
{
    
    public static IApplicationBuilder UseRequestInterceptor(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AppInterceptor>();
    }
}