using Multitenancy.Common.Multitenancy;

namespace Multitenancy.Data.Master.Entities;

public class Invoice : IHasTenantId
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public DateTime Date { get; set; }
    public int Amount { get; set; }
}
