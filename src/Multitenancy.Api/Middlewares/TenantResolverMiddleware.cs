namespace Multitenancy.Api.Middlewares;

public class TenantResolverMiddleware : IMiddleware
{
    public TenantResolverMiddleware()
    {

    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {

        await next(context);
    }
}
