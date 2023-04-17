using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Multitenancy.Common.Constants;
using Multitenancy.Services.Options;
using System.Security.Cryptography;

namespace Multitenancy.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtTokenOptions.Section).Get<JwtTokenOptions>();

        var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(
            source: Convert.FromBase64String(jwtOptions.PrivateKey),
            bytesRead: out int _
        );
        var securityKey = new RsaSecurityKey(rsa);
        services.AddSingleton(securityKey);

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = AuthenticationSchemes.Bearer;
        }).AddJwtBearer(AuthenticationSchemes.Bearer, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }

    public static IServiceCollection AddSwaggerLayer(this IServiceCollection services)
    {
        services.AddSwaggerGen(
           options =>
           {
               options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
               {
                   Name = "Authorization",
                   Type = SecuritySchemeType.ApiKey,
                   BearerFormat = "JWT",
                   In = ParameterLocation.Header,
                   Description = "JWT Authorization header using the Bearer scheme."
               });
               options.AddSecurityRequirement(new OpenApiSecurityRequirement()
               {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "",
                                Name = "Bearer",
                                In = ParameterLocation.Header,
                            },
                            new List<string>()
                        }
               });
           });

        return services;
    }
}
