using Microsoft.IdentityModel.Tokens;
using Multitenancy.Common.Constants;
using Multitenancy.Data.Master.Entities;
using Multitenancy.Services.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Multitenancy.Services.Impl;

public class TokenService : ITokenService
{
    private readonly RsaSecurityKey _rsaSecurityKey;

    public TokenService(RsaSecurityKey rsaSecurityKey)
    {
        _rsaSecurityKey = rsaSecurityKey;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ApplicationClaims.TenantId, user.TenantId.ToString())
            };

        var jwtToken = new JwtSecurityToken(issuer: "Multitenancy",
                                            audience: "Anyone",
                                            claims: claims,
                                            notBefore: DateTime.UtcNow,
                                            expires: DateTime.UtcNow.AddMinutes(90),
                                            signingCredentials: new SigningCredentials(_rsaSecurityKey, SecurityAlgorithms.RsaSha256)
                                            );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}
