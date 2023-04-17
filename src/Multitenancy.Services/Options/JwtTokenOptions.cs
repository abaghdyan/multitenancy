namespace Multitenancy.Services.Options;

public class JwtTokenOptions
{
    public const string Section = "JwtToken";

    public string PrivateKey { get; set; }
    public int AccessTokenDurationInMinutes { get; set; }
    public int AccessTokenDurationInMinutesRememberMe { get; set; }
}
