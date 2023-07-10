using Microsoft.AspNetCore.Authentication;
using Multitenancy.Common.Constants;
using Multitenancy.Common.Multitenancy;
using Multitenancy.Services.Abstractions;
using Multitenancy.Services.Models.RateLimiting;

namespace Multitenancy.Api.Middlewares;

public class RateLimitMiddleware : IMiddleware
{
    private readonly UserContext _userContext;
    private readonly IPlanLimitationService _planLimitationService;

    public RateLimitMiddleware(UserContext userContext,
        IPlanLimitationService planLimitationService)
    {
        _userContext = userContext;
        _planLimitationService = planLimitationService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var authenticateResultFeature = context.Features.Get<IAuthenticateResultFeature>();
        var authenticationTicket = authenticateResultFeature?.AuthenticateResult?.Ticket;

        if (authenticationTicket?.AuthenticationScheme != ApplicationAuthSchemes.TenantBearer)
        {
            await next(context);
            return;
        }

        var tenantId = _userContext.TenantId ?? throw new Exception("Something went wrong.");
        if (await IsRateLimitExceededAsync(context.Request.Path, tenantId))
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded!");
            return;
        }

        await next(context);
        await _planLimitationService.IncrementTenantRequestCountAsync(tenantId);
    }

    private async Task<bool> IsRateLimitExceededAsync(string endPointName, int tenantId)
    {
        var plan = new LimitationPlan
        {
            RateLimit = new RateLimitModel { RequestCount = 100, TimeWindowInSec = 10 },
            RequestLimit = new RequestLimitModel { Quantity = 10000 }
        };

        var isLimitExceeded = await _planLimitationService.IsRateLimitExceededAsync(endPointName, tenantId, plan.RateLimit);

        return isLimitExceeded;
    }
}
