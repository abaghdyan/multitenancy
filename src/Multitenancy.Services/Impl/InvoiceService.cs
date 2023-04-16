using Microsoft.EntityFrameworkCore;
using Multitenancy.Data.Tenant.Entities;
using Plat.Analytics.Data.Tenant;

namespace Multitenancy.Services.Abstractions
{
    public class InvoiceService : IInvoiceService
    {
        private readonly TenantDbContext tenantDbContext;

        public InvoiceService(TenantDbContext tenantDbContext)
        {
            this.tenantDbContext = tenantDbContext;
        }

        public async Task<List<Invoice>> GetInvoices()
        {
            var invoices = await tenantDbContext.Invoices.ToListAsync();
            return invoices;    
        }
    }
}
