using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Multitenancy.Common.Multitenancy;
using Multitenancy.Data.Master;
using Multitenancy.Data.Master.Entities;
using Multitenancy.Data.Master.Helpers;
using Multitenancy.Data.Tenant;
using Multitenancy.Services.Abstractions;
using Multitenancy.Services.Options;
using Serilog;
using System.Diagnostics;

namespace Multitenancy.Services.Impl;

public class DataTransferService : IDataTransferService
{
    private Tenant _tenant = null!;

    private readonly ILogger _logger;
    private readonly MasterDbContext _masterDbContext;
    private readonly TenantDbContext _oldDbContext;
    private readonly MasterDbOptions _options;
    private readonly IServiceProvider _serviceProvider;

    public DataTransferService(ILogger logger,
                               MasterDbContext masterDbContext,
                               TenantDbContext tenantDbContext,
                               IOptions<MasterDbOptions> options,
                               IServiceProvider serviceProvider)
    {
        _logger = logger.ForContext<DataTransferService>();
        _masterDbContext = masterDbContext;
        _options = options.Value;
        _serviceProvider = serviceProvider;
        _oldDbContext = tenantDbContext;
    }

    public async Task TransferDataAsync(int tenantId, int newStorageId)
    {
        _logger.Information($"Tenant with {tenantId} Id requested to transfer Data from Old to New storage.");
        var stopwatch = Stopwatch.StartNew();

        _tenant = await _masterDbContext.Tenants
            .Include(x => x.TenantStorage)
            .FirstAsync(x => x.Id == tenantId);

        if (_tenant == null)
        {
            throw new Exception($"Tenant with id {tenantId} not found");
        }

        if (_tenant.TenantStorageId == newStorageId)
        {
            throw new Exception($"Tenant is already in storage with {_tenant.TenantStorageId} Id");
        }

        _oldDbContext.Database.SetCommandTimeout(TimeSpan.FromSeconds(120));

        await TransferDataToNewStorageAsync(newStorageId);

        try
        {
            await DeleteDataFromOldStorageAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"{nameof(DeleteDataFromOldStorageAsync)} failed for {_tenant.Name} tenant" +
                $"with {tenantId} Id");
        }

        _logger.Information($"Tenant with {_tenant.Id} Id was successfully" +
            $" upgrated to professional in {stopwatch.Elapsed.Seconds} seconds.");
    }

    private async Task TransferDataToNewStorageAsync(int newStorageId)
    {
        _logger.Information($"Data transfering from Old to New storage started for Tenant with {_tenant.Id} Id.");

        var newStorage = await _masterDbContext.TenantStorages.FirstOrDefaultAsync(ts => ts.Id == newStorageId);

        if (newStorage == null)
        {
            throw new Exception($"Storage with {newStorageId} was not found");
        }

        using (var scope = _serviceProvider.CreateScope())
        {
            var userContext = scope.ServiceProvider.GetRequiredService<UserContext>();
            var connectionBuilder = ConnectionHelper.GetConnectionBuilder(_options.EncryptionKey, newStorage);
            userContext.SetTenantInfo(_tenant.Id, null, connectionBuilder.ToString());

            var newTenantDbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
            newTenantDbContext.Database.SetCommandTimeout(TimeSpan.FromSeconds(120));
            using var newTenantDbTransaction = await newTenantDbContext.Database.BeginTransactionAsync();

            var requiredEntities = await GetRequiredEntitiesAsync();

            RemoveAllPrimaryKeys(requiredEntities);

            foreach (var requiredEntity in requiredEntities)
            {
                newTenantDbContext.AddRange(requiredEntity);
            }

            await newTenantDbContext.SaveChangesAsync();

            try
            {
                await newTenantDbTransaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await newTenantDbTransaction.RollbackAsync();
                _logger.Error(ex, $"Error transfering {_tenant.Id} Tenant's data from Old to New storage");
                throw;
            }
        }

        using var masterDbTransaction = await _masterDbContext.Database.BeginTransactionAsync();

        //Update Master Storage
        _tenant.TenantStorageId = newStorage.Id;
        _masterDbContext.Tenants.Update(_tenant);

        await _masterDbContext.SaveChangesAsync();

        try
        {
            await masterDbTransaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await masterDbTransaction.RollbackAsync();
            _logger.Error(ex, $"Error updating {_tenant.Id} Tenant's data in Master storage while transfering from Old to New storage");
            throw;
        }

        _logger.Information($"Data transfering from Old to New storage for Tenant with {_tenant.Id} Id finished successfully.");
    }

    private async Task DeleteDataFromOldStorageAsync()
    {
        _logger.Information($"Data deletion from Old storage started for Tenant with {_tenant.Id} Id.");

        using var sourceTransaction = await _oldDbContext.Database.BeginTransactionAsync();

        var requiredEntities = await GetRequiredEntitiesAsync();

        foreach (var requiredEntity in requiredEntities)
        {
            _oldDbContext.RemoveRange(requiredEntity);
        }

        try
        {
            await _oldDbContext.SaveChangesAsync();
            await sourceTransaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await sourceTransaction.RollbackAsync();
            _logger.Error(ex, $"Error deleting {_tenant.Id} Tenant's data from Old Database");
        }

        _logger.Information($"Data deletion from Old storage for Tenant with {_tenant.Id} Id finished successfully.");
    }

    private async Task<List<IEnumerable<IHasTenantId>>> GetRequiredEntitiesAsync()
    {
        var list = new List<IEnumerable<IHasTenantId>>();
        _oldDbContext.ChangeTracker.Clear();

        var allEntities = _oldDbContext.Model.GetEntityTypes();

        foreach (var entity in allEntities)
        {
            if (entity.ClrType.IsAssignableTo(typeof(IHasTenantId)))
            {
                //var aaa = await _oldDbContext.FromExpression<IHasTenantId>(a => true).ToListAsync();
            }
        }

        return list;
    }

    private void RemoveAllPrimaryKeys(List<IEnumerable<IHasTenantId>> entities)
    {
    }
}

public static class Ext
{
    public static IQueryable<object> Set(this DbContext context, Type T)
    {
        var method = typeof(DbContext).GetMethods().Single(p =>
            p.Name == nameof(DbContext.Set) && p.ContainsGenericParameters && !p.GetParameters().Any());

        // Build a method with the specific type argument you're interested in
        method = method.MakeGenericMethod(T);

        return method.Invoke(context, null) as IQueryable<object>;
    }
}