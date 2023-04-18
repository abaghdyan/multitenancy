using Multitenancy.Common.Multitenancy;

namespace Multitenancy.Data.Master.Entities;

public partial class User : IHasTenantId
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime RegistrationDate { get; set; }
    public int TenantId { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
}
