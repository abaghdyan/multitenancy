using Multitenancy.Common.Data.Entities;

namespace Multitenancy.Common.Data.Services;

public interface ITenantDbHelper
{
    int GenerateUniqueId(AbstractEntity abstractEntity);
}
