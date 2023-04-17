using Multitenancy.Data.Master.Entities;

namespace Multitenancy.Services.Abstractions;

public interface IInvoiceService
{
    Task<List<Invoice>> GetInvoicesAsync();
    Task<Invoice?> GetInvoiceByIdAsync(int id);
}
