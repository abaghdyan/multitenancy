using Microsoft.AspNetCore.Mvc;
using Multitenancy.Api.Controllers.Base;
using Multitenancy.Common.Multitenancy;
using Multitenancy.Services.Abstractions;
using Multitenancy.Services.Impl;

namespace Multitenancy.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AdminController : MasterBaseController
{
    private readonly ITenantService _tenantService;
    private readonly IDataTransferService _dataTransferService;

    public AdminController(ITenantService tenantService,
        IDataTransferService dataTransferService)
    {
        _tenantService = tenantService;
        _dataTransferService = dataTransferService;
    }

    [HttpPost("createDemoTenants")]
    public async Task<IActionResult> CreateDemoTenants()
    {
        await _tenantService.CreateDemoTenantsAsync();
        return Ok();
    }

    [HttpPost("transferTenant")]
    public async Task<IActionResult> TransferTenant(int tenantId, int newStorageId)
    {
        await _tenantService.InitializeTenantForScopeAsync(tenantId);
        await _dataTransferService.TransferDataAsync(tenantId, newStorageId);
        return Ok();
    }
}