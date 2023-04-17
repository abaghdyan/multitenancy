using Multitenancy.Common.Multitenancy;

namespace Multitenancy.Data.Tenant.Entities;

public class Book : IHasTenantId
{
    public int Id { get; set; }
    public string TenantId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Author { get; set; } = null!;
    public int PageCount { get; set; }
}
