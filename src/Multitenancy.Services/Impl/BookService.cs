using Microsoft.EntityFrameworkCore;
using Multitenancy.Data.Tenant;
using Multitenancy.Data.Tenant.Entities;
using Multitenancy.Services.Abstractions;

namespace Multitenancy.Services.Impl;

public class BookService : IBookService
{
    private readonly TenantDbContext _tenantDbContext;

    public BookService(TenantDbContext tenantDbContext)
    {
        _tenantDbContext = tenantDbContext;
    }

    public async Task<List<Book>> GetBooksAsync()
    {
        var books = await _tenantDbContext.Books.ToListAsync();
        return books;
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        var book = await _tenantDbContext.Books.FirstOrDefaultAsync(i => i.Id == id);
        return book;
    }
}
