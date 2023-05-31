using Multitenancy.Common.Multitenancy;

namespace Multitenancy.Data.Tenant.Entities;

public class Book : AbstractEntity, IHasTenantId
{
    public int TenantId { get; set; }
    public string Name { get; set; } = null!;
    public string Author { get; set; } = null!;
    public int PageCount { get; set; }
}
