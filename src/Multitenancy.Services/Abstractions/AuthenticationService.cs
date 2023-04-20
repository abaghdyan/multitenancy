using Multitenancy.Data.Master.Entities;
using Multitenancy.Services.Models;

namespace Multitenancy.Services.Abstractions;

public interface IAuthenticationService
{
    Task<User?> SignInUserAsync(UserSignInModel userSignInModel);
    string GenerateAccessToken(User user);
}
