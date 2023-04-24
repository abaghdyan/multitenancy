using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Multitenancy.Common.Data.Helpers;
using Multitenancy.Common.Multitenancy;
using Multitenancy.Data.Master;
using Multitenancy.Data.Master.Entities;
using Multitenancy.Data.Master.Helpers;
using Multitenancy.Data.Tenant;
using Multitenancy.Services.Abstractions;
using Multitenancy.Services.Options;

namespace Multitenancy.Services.Impl;

public class TenantService : ITenantService
{
    private readonly UserContext _userContext;
    private readonly MasterDbOptions _options;
    private readonly MasterDbContext _masterDbContext;
    private readonly IServiceProvider _serviceProvider;

    public TenantService(UserContext userContext,
        MasterDbContext masterDbContext,
        IServiceProvider serviceProvider,
        IOptions<MasterDbOptions> options)
    {
        _options = options.Value;
        _userContext = userContext;
        _masterDbContext = masterDbContext;
        _serviceProvider = serviceProvider;
    }

    public async Task CreateDemoTenantsAsync()
    {
        var connectionParams = "{\"MaxPoolSize\": \"100\",\r\n \"MinPoolSize\": \"1\",\r\n \"Pooling\": \"True\",\r\n \"LoadBalanceTimeout\": \"30\",\r\n \"ConnectTimeout\": \"30\",\r\n \"TrustServerCertificate\": \"False\",\r\n \"Encrypt\": \"False\",\r\n \"MultipleActiveResultSets\": \"False\",\r\n \"PersistSecurityInfo\": \"False\"}";

        using var masterDbTransaction = await _masterDbContext.Database.BeginTransactionAsync();

        var firstStorage = new TenantStorage
        {
            StorageName = "FirstStorage",
            Database = SecurityHelper.Encrypt(_options.EncryptionKey, "Local-Tenant-Storage-1"),
            Server = SecurityHelper.Encrypt(_options.EncryptionKey, "(localdb)\\MSSQLLocalDB"),
            Username = "",
            Password = "",
            Location = "WUS",
            Status = "Active",
            ConnectionParameters = connectionParams
        };
        _masterDbContext.TenantStorages.Add(firstStorage);

        var secondStorage = new TenantStorage
        {
            StorageName = "SecondStorage",
            Database = SecurityHelper.Encrypt(_options.EncryptionKey, "Local-Tenant-Storage-2"),
            Server = SecurityHelper.Encrypt(_options.EncryptionKey, "(localdb)\\MSSQLLocalDB"),
            Username = "",
            Password = "",
            Location = "WUS",
            Status = "Active",
            ConnectionParameters = connectionParams
        };
        _masterDbContext.TenantStorages.Add(secondStorage);

        await _masterDbContext.SaveChangesAsync();

        var tenantModels = new List<Tenant>()
        {
            new Tenant { Name = "tenant1",  TenantStorageId = firstStorage.Id},
            new Tenant { Name = "tenant2", TenantStorageId = secondStorage.Id},
            new Tenant { Name = "tenant3", TenantStorageId = secondStorage.Id},
        };

        foreach (var tenantModel in tenantModels)
        {
            var storage = _masterDbContext.TenantStorages
                .Include(ts => ts.Tenants)
                .First(ts => ts.Id == tenantModel.TenantStorageId);
            var tenant = new Tenant
            {
                Name = tenantModel.Name,
                TenantStorageId = tenantModel.TenantStorageId,
                Status = "Active",
                CreatedOn = DateTime.UtcNow
            };
            _masterDbContext.Tenants.Add(tenant);

            var users = new List<User>
            {
                new User
                {
                    Tenant = tenant,
                    Name = $"Admin_{tenant.Name}",
                    Email = $"admin@{tenant.Name}.com",
                    Status = "Active",
                    Password = "password",
                    RegistrationDate = DateTime.UtcNow,
                },
                new User
                {
                    Tenant = tenant,
                    Name = $"User{tenant.Name}",
                    Email = $"user@{tenant.Name}.com",
                    Status = "Active",
                    Password = "password",
                    RegistrationDate = DateTime.UtcNow,
                }
            };
            _masterDbContext.Users.AddRange(users);

            await _masterDbContext.SaveChangesAsync();

            var scope = _serviceProvider.CreateScope();
            var userContext = scope.ServiceProvider.GetRequiredService<UserContext>();
            var connectionBuilder = ConnectionHelper.GetConnectionBuilder(_options.EncryptionKey, storage);
            userContext.SetConnectionString(connectionBuilder.ToString());

            var newTenantDbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

            await newTenantDbContext.Database.MigrateAsync();

            await newTenantDbContext.SaveChangesAsync();
        }

        await masterDbTransaction.CommitAsync();
    }

    public async Task<Tenant> InitializeTenantForScopeAsync(int tenantId)
    {
        var tenant = await _masterDbContext.Tenants
            .Include(t => t.TenantStorage)
            .Where(t => t.Id == tenantId)
            .FirstOrDefaultAsync();

        if (tenant == null)
        {
            throw new ArgumentNullException($"Tenant with {tenantId} Id was not found.");
        }

        var connectionBuilder = ConnectionHelper.GetConnectionBuilder(_options.EncryptionKey, tenant.TenantStorage);
        _userContext.SetTenantInfo(tenantId, null,  connectionBuilder.ToString());
        
        return tenant;
    }
}
