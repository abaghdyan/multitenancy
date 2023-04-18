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
        var authHeader = context.Request?.Headers["Authorization"].ToString();
        var token = authHeader?.Replace("Bearer ", string.Empty).Replace("bearer ", string.Empty);

        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);

        var userIdClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        var tenantIdClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ApplicationClaims.TenantId);

        if (userIdClaim == null)
        {
            throw new ArgumentNullException($"Invalid UserId");
        }

        if (tenantIdClaim == null)
        {
            throw new ArgumentNullException($"Invalid TenantId");
        }

        var userId = userIdClaim.Value;
        var tenantId = tenantIdClaim.Value;


        var tenant = await _masterDbContext.Tenants
            .Include(t => t.TenantStorage)
            .Where(t => t.Id == tenantId)
            .FirstOrDefaultAsync();

        if (tenant == null)
        {
            throw new ArgumentNullException($"Tenant with {tenantId} Id was not found.");
        }

        var connectionBuilder = ConnectionHelper.GetConnectionBuilder(_options.EncryptionKey, tenant.TenantStorage);
        _userContext.SetTenantInfo(tenantIdClaim.Value, userIdClaim.Value, connectionBuilder.ToString());

        await next(context);
    }
}
