using System.Diagnostics;

namespace Multitenancy.Api.Middlewares;

public class GlobalExceptionHandler : IMiddleware
{
    private readonly ILogger _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandler(ILogger logger,
        IWebHostEnvironment environment)
    {
        _environment = environment;
        _logger = logger.ForContext<GlobalExceptionHandler>();
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var traceMessage = $"Request trace id: {Activity.Current?.TraceId.ToString()}";

        var message = $"Something went wrong. {traceMessage}";

        if (!_environment.IsProduction())
        {
            message += $" Exception message: {ex?.InnerException?.Message ?? ex?.Message ?? ""}";
        }

        _logger.Error(ex, "Log from Global Exception Handler.", ex?.StackTrace);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return context.Response.WriteAsync(message);
    }
}
