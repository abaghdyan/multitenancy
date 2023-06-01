using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Multitenancy.Common.Data.Entities;
using Multitenancy.Common.Data.Services;
using Multitenancy.Common.Multitenancy;
using Multitenancy.Data.Master.Entities;
using System.Linq.Expressions;

namespace Multitenancy.Data.Master;

public partial class MasterDbContext : DbContext
{
    private readonly UserContext _userContext;
    private readonly ITenantDbHelper _tenantDbHelper;

    public MasterDbContext(DbContextOptions<MasterDbContext> options,
        UserContext userContext,
        ITenantDbHelper tenantDbHelper)
        : base(options)
    {
        _userContext = userContext;
        _tenantDbHelper = tenantDbHelper;
    }

    public virtual DbSet<Tenant> Tenants { get; set; } = null!;
    public virtual DbSet<TenantStorage> TenantStorages { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<Invoice> Invoices { get; set; } = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var addedEntities = ChangeTracker
                .Entries()
                .Where(p => p.State == EntityState.Added)
                .ToList();

        foreach (var addedEntity in addedEntities)
        {
            var abstractEntity = addedEntity.Entity as AbstractEntity;
            if (abstractEntity != null)
            {
                abstractEntity.Id = _tenantDbHelper.GenerateUniqueId(abstractEntity);
            }
        }

        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Setting global query filter for TenantId property
        Expression<Func<IHasTenantId, bool>> filterExpr = e => e.TenantId == _userContext.TenantId;
        foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
        {
            if (mutableEntityType.ClrType.IsAssignableTo(typeof(IHasTenantId)))
            {
                var parameter = Expression.Parameter(mutableEntityType.ClrType);
                var body = ReplacingExpressionVisitor.Replace(filterExpr.Parameters.First(), parameter, filterExpr.Body);
                var lambdaExpression = Expression.Lambda(body, parameter);
                mutableEntityType.SetQueryFilter(lambdaExpression);
            }
        }

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasOne(d => d.TenantStorage)
                .WithMany(p => p.Tenants)
                .HasForeignKey(d => d.TenantStorageId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasOne(d => d.Tenant)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.TenantId);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(x => new { x.Id, x.TenantId });
        });
    }
}
