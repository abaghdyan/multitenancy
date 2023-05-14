namespace Multitenancy.Api.Options;

public class JwtTokenOptions
{
    public const string Section = "JwtToken";

    public string PrivateKey { get; set; } = null!;
}
