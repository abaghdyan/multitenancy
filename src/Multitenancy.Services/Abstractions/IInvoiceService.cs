using Multitenancy.Data.Tenant.Entities;

namespace Multitenancy.Services.Abstractions
{
    public interface IInvoiceService
    {
        Task<List<Invoice>> GetInvoices();
    }
}
