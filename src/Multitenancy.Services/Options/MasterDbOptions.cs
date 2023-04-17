namespace Multitenancy.Services.Options;

public class MasterDbOptions
{
    public const string Section = "MasterDb";

    public string ConnectionString { get; set; } = null!;
    public string EncryptionKey { get; set; } = null!;
}
