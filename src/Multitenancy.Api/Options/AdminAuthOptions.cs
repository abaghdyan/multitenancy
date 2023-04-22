namespace Multitenancy.Api.Options;

public class AdminAuthOptions
{
    public const string Section = "AdminAuth";

    public string ApiKeyValue { get; set; } = null!;
}
