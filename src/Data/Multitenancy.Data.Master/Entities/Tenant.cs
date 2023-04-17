namespace Multitenancy.Data.Master.Entities;

public class Tenant
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public int TenantStorageId { get; set; }

    public TenantStorage TenantStorage { get; set; } = null!;
    public virtual ICollection<User> Users { get; set; } = null!;
}
