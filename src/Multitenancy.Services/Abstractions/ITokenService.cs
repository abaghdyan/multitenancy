using Multitenancy.Data.Master.Entities;

namespace Multitenancy.Services.Abstractions;

public interface ITokenService
{
    string GenerateAccessToken(User user);
}
