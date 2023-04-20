using Multitenancy.Data.Master.Entities;
using Multitenancy.Services.Models;

namespace Multitenancy.Services.Abstractions;

public interface IInvoiceService
{
    Task<List<Invoice>> GetInvoicesAsync();
    Task<Invoice?> GetInvoiceByIdAsync(int id);
    Task AddInvoiceAsync(InvoiceInputModel invoiceInputModel);
}
