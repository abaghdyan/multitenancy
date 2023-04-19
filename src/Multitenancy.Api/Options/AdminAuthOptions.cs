namespace Multitenancy.Services.Options;

public class AdminAuthOptions
{
    public const string Section = "AdminAuth";

    public string ApiKeyValue { get; set; } = null!;
}
