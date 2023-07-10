using Multitenancy.Services.Models.RateLimiting;

namespace Multitenancy.Services.Abstractions;

public interface IPlanLimitationService
{
    Task<bool> IsRateLimitExceededAsync(string limitationScope, int tenantId, RateLimitModel plan);
    Task<bool> IsRequestLimitExceededAsync(int tenantId, RequestLimitModel plan);
    Task IncrementTenantRequestCountAsync(int tenantId);
}
