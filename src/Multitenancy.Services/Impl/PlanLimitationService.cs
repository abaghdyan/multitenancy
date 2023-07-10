using Multitenancy.Common.Constants;
using Multitenancy.Services.Abstractions;
using Multitenancy.Services.Constants;
using Multitenancy.Services.Models.RateLimiting;
using Serilog;
using static Multitenancy.Common.Constants.CacheConstants;

namespace Multitenancy.Services.Impl;

public class PlanLimitationService : IPlanLimitationService
{
    private readonly CacheConnector _cacheConnector;
    private readonly ILogger _logger;

    public PlanLimitationService(CacheConnector cacheConnector, ILogger logger)
    {
        _cacheConnector = cacheConnector;
        _logger = logger.ForContext<PlanLimitationService>();
    }

    public async Task<bool> IsRateLimitExceededAsync(string limitationScope, int tenantId, RateLimitModel rateLimit)
    {
        if (rateLimit == null)
            return false;

        var tenantRateLimitKey = PlanLimitation.TenantRateLimitationById.GetKey(tenantId, limitationScope);
        var limited = (int)await _cacheConnector.RedisDatabase.ScriptEvaluateAsync(RedisScripts.SlidingRateLimiterScript,
            new { key = tenantRateLimitKey, window = rateLimit.TimeWindowInSec, max_requests = rateLimit.RequestCount }) == 1;

        return limited;
    }

    public async Task<bool> IsRequestLimitExceededAsync(int tenantId, RequestLimitModel requestLimit)
    {
        if (requestLimit == null)
        {
            _logger.Error($"Request limit not found, TenantId: {tenantId}");
            return false;
        }
        var tenantRequestLimitKey = PlanLimitation.TenantRequestLimitationById.GetKey(tenantId);
        var tenantRequestMaxUsageKey = PlanLimitation.TenantRequestMaxUsageById.GetKey(tenantId);
        var limited = (int)await _cacheConnector.RedisDatabase.ScriptEvaluateAsync(RedisScripts.SlidingRequestLimiterScript,
            new
            {
                key = tenantRequestLimitKey,
                maxUsageKey = tenantRequestMaxUsageKey,
                maxCount = requestLimit.Quantity,
                allowScale = requestLimit.AllowScale
            }) == 1;

        return limited;
    }

    public async Task IncrementTenantRequestCountAsync(int tenantId)
    {
        var key = PlanLimitation.TenantRequestLimitationById.GetKey(tenantId);
        await _cacheConnector.RedisDatabase.ScriptEvaluateAsync(RedisScripts.SlidingIncrementScript,
                                                                new { key });
    }
}
