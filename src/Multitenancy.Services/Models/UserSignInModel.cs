namespace Multitenancy.Services.Models;

public class UserSignInModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
