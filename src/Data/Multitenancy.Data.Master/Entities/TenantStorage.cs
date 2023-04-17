namespace Multitenancy.Data.Master.Entities;

public class TenantStorage
{
    public int Id { get; set; }
    public string StorageName { get; set; } = null!;
    public string Server { get; set; } = null!;
    public string Database { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Location { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? ConnectionParameters { get; set; }

    public virtual ICollection<Tenant>? Tenants { get; set; }
}
