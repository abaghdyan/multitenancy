using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Multitenancy.Common.Constants;
using Multitenancy.Common.Multitenancy;
using Multitenancy.Data.Master;
using Multitenancy.Data.Master.Helpers;
using Multitenancy.Services.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Multitenancy.Api.Middlewares;

public class TenantResolverMiddleware : IMiddleware
{
    private readonly UserContext _userContext;
    private readonly MasterDbContext _masterDbContext;
    private readonly MasterDbOptions _options;

    public TenantResolverMiddleware(UserContext userContext,
        MasterDbContext masterDbContext,
        IOptions<MasterDbOptions> options)
    {
        _options = options.Value;
        _userContext = userContext;
        _masterDbContext = masterDbContext;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var authenticateResultFeature = context.Features.Get<IAuthenticateResultFeature>();
        var authenticationTicket = authenticateResultFeature?.AuthenticateResult?.Ticket;

        if (authenticationTicket?.AuthenticationScheme != ApplicationAuthSchemes.TenantBearer)
        {
            await next(context);
            return;
        }

        var authHeader = context.Request?.Headers.Authorization.ToString();
        var token = authHeader?.Replace("Bearer ", string.Empty).Replace("bearer ", string.Empty);

        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);

        var userIdClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ApplicationClaims.UserId);
        var tenantIdClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ApplicationClaims.TenantId);

        if (!int.TryParse(userIdClaim?.Value, out var userId))
        {
            throw new Exception("Invalid UserId");
        }

        if (!int.TryParse(tenantIdClaim?.Value, out var tenantId))
        {
            throw new Exception("Invalid TenantId");
        }

        var tenant = await _masterDbContext.Tenants
            .Include(t => t.TenantStorage)
            .Where(t => t.Id == tenantId)
            .FirstOrDefaultAsync();

        if (tenant == null)
        {
            throw new ArgumentNullException($"Tenant with {tenantId} Id was not found.");
        }

        var connectionBuilder = ConnectionHelper.GetConnectionBuilder(_options.EncryptionKey, tenant.TenantStorage);
        _userContext.SetTenantInfo(tenantId, userId, connectionBuilder.ToString());

        await next(context);
    }
}
