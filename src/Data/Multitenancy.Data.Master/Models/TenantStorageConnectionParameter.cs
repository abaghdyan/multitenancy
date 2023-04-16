namespace Plat.Analytics.Services.Models.Tenant
{
    public class TenantStorageConnectionParameter
    {
        public bool? PersistSecurityInfo { get; set; }
        public bool? MultipleActiveResultSets { get; set; }
        public bool? Encrypt { get; set; }
        public bool? TrustServerCertificate { get; set; }
        public int? ConnectTimeout { get; set; }
        public int? LoadBalanceTimeout { get; set; }
        public bool? Pooling { get; set; }
        public int? MinPoolSize { get; set; }
        public int? MaxPoolSize { get; set; }
    }
}
