using Multitenancy.Data.Tenant.Entities;

namespace Multitenancy.Data.Tenant.Services;

public interface ITenantDbHelper
{
    int GenerateUniqueId(AbstractEntity abstractEntity);
}
