namespace Multitenancy.Common.Multitenancy;

public class UserContext
{
    public int? TenantId { get; set; }
    public string? ConnectionString { get; set; }
    public int UserId { get; set; }

    public void SetTenantInfo(int? tenantId, int userId, string connectionString)
    {
        TenantId = tenantId;
        UserId = userId;
        ConnectionString = connectionString;
    }

    public void SetConnectionString(string connectionString)
    {
        ConnectionString = connectionString;
    }
}
