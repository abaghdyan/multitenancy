namespace Multitenancy.Common.Multitenancy;

public class UserContext
{
    public string? TenantId { get; set; }
    public string? ConnectionString { get; set; }
    public string UserId { get; set; }

    public void SetTenantInfo(string tenantId, string userId, string connectionString)
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
