using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Multitenancy.Common.Multitenancy;
using Multitenancy.Data.Tenant.Entities;
using System.Linq.Expressions;

namespace Multitenancy.Data.Tenant;

public partial class TenantDbContext : DbContext
{
    private readonly UserContext _userContext;

    public TenantDbContext(DbContextOptions<TenantDbContext> options,
        UserContext userContext)
        : base(options)
    {
        _userContext = userContext;
    }

    public virtual DbSet<Book> Books { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
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
    }
}
