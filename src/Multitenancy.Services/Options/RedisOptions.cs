namespace Multitenancy.Services.Options;

public class RedisOptions
{
    public const string Section = "Redis";

    public int DatabaseNumber { get; set; } = 1;
    public string ConnectionString { get; set; } = null!;
}