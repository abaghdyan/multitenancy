using Microsoft.EntityFrameworkCore;
using Multitenancy.Data.Master;
using Multitenancy.Data.Master.Entities;
using Multitenancy.Services.Abstractions;

namespace Multitenancy.Services.Impl;

public class InvoiceService : IInvoiceService
{
    private readonly MasterDbContext _masterDbContext;

    public InvoiceService(MasterDbContext masterDbContext)
    {
        _masterDbContext = masterDbContext;
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
}
