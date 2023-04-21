using Multitenancy.Data.Tenant.Entities;
using Multitenancy.Services.Models;

namespace Multitenancy.Services.Abstractions;

public interface IBookService
{
    Task<List<Book>> GetBooksAsync();
    Task<Book?> GetBookByIdAsync(int id);
    Task AddBookAsync(BookInputModel bookInputModel);
}
