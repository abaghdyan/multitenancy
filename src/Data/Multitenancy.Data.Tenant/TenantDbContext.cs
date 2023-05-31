using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Multitenancy.Common.Multitenancy;
using Multitenancy.Data.Tenant.Entities;
using Multitenancy.Data.Tenant.Services;
using System.Linq.Expressions;

namespace Multitenancy.Data.Tenant;

public partial class TenantDbContext : DbContext
{
    private readonly UserContext _userContext;
    private readonly ITenantDbHelper _tenantDbHelper;

    public TenantDbContext(DbContextOptions<TenantDbContext> options,
        ITenantDbHelper tenantDbHelper,
        UserContext userContext)
        : base(options)
    {
        _userContext = userContext;
        _tenantDbHelper = tenantDbHelper;
    }

    public virtual DbSet<Book> Books { get; set; } = null!;

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            if (!string.IsNullOrEmpty(_userContext.ConnectionString))
            {
                optionsBuilder.UseSqlServer(_userContext.ConnectionString);
            }
            else
            {
                throw new InvalidOperationException("Tenant related database connection string was not initialized");
            }
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

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(x => new { x.Id, x.TenantId });
        });
    }
}
