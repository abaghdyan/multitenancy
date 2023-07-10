namespace Multitenancy.Common.Constants;

public static class CacheConstants
{
    #region PlanLimitation
    public static class PlanLimitation
    {
        public const string TenantRateLimitationById = "{0}-Cache.PlanLimitation.TenantRateLimitationById-{1}";
        public const string TenantRequestLimitationById = "{0}-Cache.PlanLimitation.TenantRequestLimitationById";
        public const string TenantRequestMaxUsageById = "{0}-Cache.PlanLimitation.TenantRequestMaxUsageById";
    }
    #endregion
    #region Tenant
    public static class Tenant
    {
        public const string PatternKey = "Cache.Tenant.";
        public const string TenantPatternKey = "{0}-Cache.Tenant.";
        public const string GetTenantIncludedOtherEnitiesById = "{0}-Cache.Tenant.GetTenantIncludedOtherEnitiesById";
        public const string GetTenantById = "{0}-Cache.Tenant.GetTenantById";
    }
    #endregion

    public static string GetKey(this string key, params object?[] args)
    {
        return string.Format(key, args);
    }
}
