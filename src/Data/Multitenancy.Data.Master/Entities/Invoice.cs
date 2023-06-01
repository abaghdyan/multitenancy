using Multitenancy.Common.Data.Entities;
using Multitenancy.Common.Multitenancy;

namespace Multitenancy.Data.Master.Entities;

public class Invoice : AbstractEntity, IHasTenantId
{
    public int TenantId { get; set; }
    public DateTime Date { get; set; }
    public int Amount { get; set; }
}
