using Multitenancy.Common.Data.Entities;

namespace Multitenancy.Common.Data.Services;

public class TenantDbHelper : ITenantDbHelper
{
    public int GenerateUniqueId(AbstractEntity abstractEntity)
    {
        return new Random().Next();
    }
}
