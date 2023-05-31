using Multitenancy.Data.Tenant.Entities;

namespace Multitenancy.Data.Tenant.Services;

public class TenantDbHelper : ITenantDbHelper
{
    public int GenerateUniqueId(AbstractEntity abstractEntity)
    {
        return new Random().Next();
    }
}
