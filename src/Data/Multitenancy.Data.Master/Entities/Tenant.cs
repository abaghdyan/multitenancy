namespace Plat.Analytics.Data.Master
{
    public class Tenant
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public int TenantStorageId { get; set; }

        public TenantStorage? TenantStorage { get; set; }
        public virtual ICollection<User> Users { get; set; } = null!;
    }
}
