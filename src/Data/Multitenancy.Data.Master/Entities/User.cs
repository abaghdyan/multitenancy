namespace Plat.Analytics.Data.Master
{
    public partial class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string  Status { get; set; } = null!;
        public DateTime RegistrationDate { get; set; }
        public long TenantId { get; set; }

        public virtual Tenant Tenant { get; set; } = null!;
    }
}
