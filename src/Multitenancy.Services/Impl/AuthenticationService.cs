using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Multitenancy.Common.Constants;
using Multitenancy.Data.Master;
using Multitenancy.Data.Master.Entities;
using Multitenancy.Services.Abstractions;
using Multitenancy.Services.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Multitenancy.Services.Impl;

public class AuthenticationService : IAuthenticationService
{
    private readonly MasterDbContext _masterDbContext;
    private readonly RsaSecurityKey _rsaSecurityKey;

    public AuthenticationService(MasterDbContext masterDbContext, RsaSecurityKey rsaSecurityKey)
    {
        _masterDbContext = masterDbContext;
        _rsaSecurityKey = rsaSecurityKey;
    }

    public async Task<User?> SignInUserAsync(UserSignInModel userSignInModel)
    {
        var user = await _masterDbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == userSignInModel.Email &&
                u.Password == userSignInModel.Password);

        return user;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ApplicationClaims.UserId, user.Id.ToString()),
            new Claim(ApplicationClaims.TenantId, user.TenantId.ToString())
        };

        var jwtToken = new JwtSecurityToken(issuer: "Multitenancy",
                                            audience: "Multitenancy",
                                            claims: claims,
                                            notBefore: DateTime.UtcNow,
                                            expires: DateTime.UtcNow.AddMinutes(90),
                                            signingCredentials: new SigningCredentials(_rsaSecurityKey, SecurityAlgorithms.RsaSha256)
                                            );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}
