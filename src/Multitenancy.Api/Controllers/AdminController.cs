using Microsoft.AspNetCore.Mvc;
using Multitenancy.Api.Controllers.Base;
using Multitenancy.Services.Abstractions;

namespace Multitenancy.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AdminController : MasterBaseController
{
    private readonly ITenantService _tenantService;

    public AdminController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpPost("createDemoTenants")]
    public async Task<IActionResult> CreateDemoTenants()
    {
        await _tenantService.CreateDemoTenantsAsync();
        return Ok();
    }
}