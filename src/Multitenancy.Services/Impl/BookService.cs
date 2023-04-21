using Microsoft.EntityFrameworkCore;
using Multitenancy.Common.Multitenancy;
using Multitenancy.Data.Tenant;
using Multitenancy.Data.Tenant.Entities;
using Multitenancy.Services.Abstractions;
using Multitenancy.Services.Models;

namespace Multitenancy.Services.Impl;

public class BookService : IBookService
{
    private readonly TenantDbContext _tenantDbContext;
    private readonly UserContext _userContext;

    public BookService(TenantDbContext tenantDbContext,
        UserContext userContext)
    {
        _tenantDbContext = tenantDbContext;
        _userContext = userContext;
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

    public async Task AddBookAsync(BookInputModel bookInputModel)
    {
        var book = new Book
        {
            TenantId = _userContext.GetRequiredTenantId(),
            Name = bookInputModel.Name,
            Author = bookInputModel.Author,
            PageCount = bookInputModel.PageCount
        };

        _tenantDbContext.Books.Add(book);
        await _tenantDbContext.SaveChangesAsync();

    }
}
