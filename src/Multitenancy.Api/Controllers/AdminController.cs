using Microsoft.AspNetCore.Mvc;
using Multitenancy.Api.Controllers.Base;
using Multitenancy.Services.Abstractions;

namespace Multitenancy.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController : MasterBaseController
{
    private readonly ITenantAllocator _tenantAllocator;

    public AdminController(ITenantAllocator tenantAllocator)
    {
        _tenantAllocator = tenantAllocator;
    }

    [HttpPost("createDemoTenants")]
    public async Task<IActionResult> CreateDemoTenants()
    {
        await _tenantAllocator.CreateDemoTenantsAsync();
        return Ok();
    }
}