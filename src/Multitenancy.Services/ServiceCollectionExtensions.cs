using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Multitenancy.Common.Multitenancy;
using Plat.Analytics.Data.Master;
using Plat.Analytics.Data.Tenant;
using Plat.Analytics.Services.Helpers;
using Plat.Analytics.Services.Options;
using System.Diagnostics;
using Serilog;

namespace Multitenancy.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMultitenancy(this IServiceCollection services)
        {
            services
                .AddScoped<UserContext>()
                .AddDbContext<TenantDbContext>((provider, cfg) =>
                {
                    var currentScopeInfo = provider.GetRequiredService<UserContext>();
                    if (!string.IsNullOrEmpty(currentScopeInfo.ConnectionString))
                    {
                        cfg.UseSqlServer(currentScopeInfo.ConnectionString);
                    }
                });

            return services;
        }

        public static IServiceCollection AddMasterDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetSection(MasterDbOptions.Section).Get<MasterDbOptions>().ConnectionString;

            services.AddDbContext<MasterDbContext>(options =>
                    options.UseSqlServer(connectionString));

            return services;
        }

        public static async Task MigrateMasterDbContextAsync(this IServiceProvider provider)
        {
            using var serviceScope = provider.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<MasterDbContext>();
            var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger>().ForContext<MasterDbContext>();
            var migrationTime = new Stopwatch();

            logger.Information("MasterDbContext Migration started");
            migrationTime.Start();
            try
            {
                await dbContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Master Database migration failed. " +
                    $"Message: {ex.Message ?? ex.InnerException?.Message}. ExceptionType: {ex.GetType().FullName}");
                throw;
            }
            migrationTime.Stop();
            logger.Information($"MasterDbContext Migrating finished", new { Duration = migrationTime.ElapsedMilliseconds });
        }

        public static async Task MigrateTenantDbContextsAsync(this IServiceProvider provider)
        {
            using var serviceScope = provider.CreateScope();
            var masterDbContext = serviceScope.ServiceProvider.GetRequiredService<MasterDbContext>();
            var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger>().ForContext<TenantDbContext>();
            var migrationTime = new Stopwatch();
            logger.Information("TenantDbContext Migration started");
            migrationTime.Start();
            var tenantStorages = await masterDbContext.TenantStorages.ToListAsync();
            foreach (var tenantStorage in tenantStorages)
            {
                try
                {
                    using var scope = provider.CreateAsyncScope();
                    var serviceProvider = scope.ServiceProvider;

                    var options = serviceProvider.GetRequiredService<IOptions<MasterDbOptions>>();
                    var userContext = serviceProvider.GetRequiredService<UserContext>();

                    var connectionBuilder = ConnectionHelper.GetConnectionBuilder(options.Value.EncryptionKey, tenantStorage);
                    userContext.SetConnectionString(connectionBuilder.ToString());

                    var tenantDbContext = serviceProvider.GetRequiredService<TenantDbContext>();

                    await tenantDbContext.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    logger.Error(ex,
                        $"'{tenantStorage.StorageName}' tenant storage migration failed. " +
                        $"Message: {ex.Message ?? ex.InnerException?.Message}. ExceptionType: {ex.GetType().FullName}");
                }
            }
            migrationTime.Stop();
            logger.Information($"All tenant storages migration finished", new { Duration = migrationTime.ElapsedMilliseconds });
        }
    }
}
