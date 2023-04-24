using Multitenancy.Data.Master.Entities;

namespace Multitenancy.Services.Abstractions;

public interface ITenantService
{
    Task CreateDemoTenantsAsync();
    Task<Tenant> InitializeTenantForScopeAsync(int tenantId);
}
