using Microsoft.AspNetCore.Mvc;
using Multitenancy.Api.Controllers.Base;
using Multitenancy.Common.Multitenancy;
using Multitenancy.Data.Master.Entities;
using Multitenancy.Data.Tenant.Entities;
using Multitenancy.Services.Abstractions;
using Multitenancy.Services.Models;

namespace Multitenancy.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BookController : TenantBaseController
{
    private readonly IBookService _bookService;

    public BookController(UserContext userContext,
        IBookService bookService)
        : base(userContext)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Book>>> GetBooks()
    {
        try
        {
            var books = await _bookService.GetBooksAsync();
            return Ok(books);
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    [HttpGet("{bookId}")]
    public async Task<ActionResult<Book>> GetBookById(int bookId)
    {
        var book = await _bookService.GetBookByIdAsync(bookId);
        return Ok(book);
    }

    [HttpPost("addBook")]
    public async Task<ActionResult<Invoice>> AddBook(BookInputModel bookInputModel)
    {
        await _bookService.AddBookAsync(bookInputModel);
        return Ok();
    }
}