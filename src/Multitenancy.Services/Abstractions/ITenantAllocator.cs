namespace Multitenancy.Services.Abstractions;

public interface ITenantAllocator
{
    Task CreateDemoTenantsAsync();
}
