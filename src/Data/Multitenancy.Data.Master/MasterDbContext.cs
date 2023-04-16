using Microsoft.EntityFrameworkCore;

namespace Plat.Analytics.Data.Master
{
    public partial class MasterDbContext : DbContext
    {
        public MasterDbContext(DbContextOptions<MasterDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Tenant> Tenants { get; set; } = null!;
        public virtual DbSet<TenantStorage> TenantStorages { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}


