using Microsoft.EntityFrameworkCore;
using Multitenancy.Common.Multitenancy;
using Multitenancy.Data.Master;
using Multitenancy.Data.Master.Entities;
using Multitenancy.Services.Abstractions;
using Multitenancy.Services.Models;

namespace Multitenancy.Services.Impl;

public class InvoiceService : IInvoiceService
{
    private readonly MasterDbContext _masterDbContext;
    private readonly UserContext _userContext;

    public InvoiceService(MasterDbContext masterDbContext, UserContext userContext)
    {
        _masterDbContext = masterDbContext;
        _userContext = userContext;
    }

    public async Task<List<Invoice>> GetInvoicesAsync()
    {
        var invoices = await _masterDbContext.Invoices.ToListAsync();
        return invoices;
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(int id)
    {
        var invoice = await _masterDbContext.Invoices.FirstOrDefaultAsync(i => i.Id == id);
        return invoice;
    }

    public async Task AddInvoiceAsync(InvoiceInputModel invoiceInputModel)
    {
        var invoice = new Invoice
        {
            TenantId = _userContext.GetRequiredTenantId(),
            Amount = invoiceInputModel.Amount,
            Date = invoiceInputModel.Date
        };

        _masterDbContext.Invoices.Add(invoice);
        await _masterDbContext.SaveChangesAsync();
    }
}
