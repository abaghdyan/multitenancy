using Multitenancy.Data.Tenant.Entities;

namespace Multitenancy.Services.Abstractions;

public interface IBookService
{
    Task<List<Book>> GetBooksAsync();
    Task<Book?> GetBookByIdAsync(int id);
}
