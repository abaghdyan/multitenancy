using Plat.Analytics.Common;

namespace Multitenancy.Data.Tenant.Entities
{
    public class Invoice : IHasTenantId
    {
        public int Id { get; set; }
        public string TenantId { get; set; } = null!;
        public DateTime Date { get; set; }
        public int Amount { get; set; }
    }
}
