using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Multitenancy.Api.Handlers;
using Multitenancy.Api.Middlewares;
using Multitenancy.Common.Constants;
using Multitenancy.Services.Options;
using System.Security.Cryptography;

namespace Multitenancy.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureApplicationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AdminAuthOptions>(configuration.GetSection(AdminAuthOptions.Section));
        services.Configure<MasterDbOptions>(configuration.GetSection(MasterDbOptions.Section));

        return services;
    }

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
            options.DefaultScheme = ApplicationAuthSchemes.TenantBearer;
        }).AddJwtBearer(ApplicationAuthSchemes.TenantBearer, options =>
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
        }).AddScheme<AuthenticationSchemeOptions, AdminAuthenticationHandler>(
                ApplicationAuthSchemes.AdminFlow, options => { });

        services.AddTransient<TenantResolverMiddleware>();

        return services;
    }

    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        builder.Logging.ClearProviders();
        builder.Host.UseSerilog();

        builder.Logging.ClearProviders();

        builder.Host.UseSerilog((_, sp, lc) =>
        {
            lc.Enrich.FromLogContext();

            lc.ReadFrom.Configuration(configuration);

            lc.Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("swagger") ||
                    p.Value.ToString().Contains("health")));
        });

        return builder;
    }

    public static IServiceCollection AddSwaggerLayer(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Multitenanct", Version = "v1" });
            c.AddSecurityDefinition("Api Key Auth", new OpenApiSecurityScheme
            {
                Description = "ApiKey must appear in header",
                Type = SecuritySchemeType.ApiKey,
                Name = ApplicationHeaders.AdminFlowKey,
                In = ParameterLocation.Header,
                Scheme = "ApiKeyScheme"
            });
            c.AddSecurityDefinition("Bearer Auth", new OpenApiSecurityScheme()
            {
                Description = $"JWT Authorization header using the {ApplicationAuthSchemes.TenantBearer} scheme.",
                Type = SecuritySchemeType.ApiKey,
                Name = "Authorization",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Scheme = ApplicationAuthSchemes.TenantBearer
            });

            var requirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Api Key Auth" }
                    },
                    new[] { "DemoSwaggerDifferentAuthScheme" }
                },
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer Auth" }
                    },
                    new[] { "DemoSwaggerDifferentAuthScheme" }
                }
            };

            c.AddSecurityRequirement(requirement);
        });

        return services;
    }
}
