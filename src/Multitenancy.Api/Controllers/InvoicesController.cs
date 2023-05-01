using Microsoft.AspNetCore.Mvc;
using Multitenancy.Api.Controllers.Base;
using Multitenancy.Common.Multitenancy;
using Multitenancy.Data.Master.Entities;
using Multitenancy.Services.Abstractions;
using Multitenancy.Services.Models;

namespace Multitenancy.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InvoicesController : TenantBaseController
{
    private readonly IInvoiceService _invoiceService;
    public InvoicesController(UserContext userContext,
        IInvoiceService invoiceService)
        : base(userContext)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Invoice>>> GetInvoices()
    {
        var invoices = await _invoiceService.GetInvoicesAsync();
        return Ok(invoices);
    }

    [HttpGet("{invoiceId}")]
    public async Task<ActionResult<Invoice>> GetInvoiceById(int invoiceId)
    {
        var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
        return Ok(invoice);
    }

    [HttpPost("addInvoice")]
    public async Task<ActionResult<Invoice>> AddInvoice(InvoiceInputModel invoiceInputModel)
    {
        await _invoiceService.AddInvoiceAsync(invoiceInputModel);
        return Ok();
    }
}