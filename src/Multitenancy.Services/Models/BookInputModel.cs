namespace Multitenancy.Services.Models;

public class BookInputModel
{
    public string Name { get; set; } = null!;
    public string Author { get; set; } = null!;
    public int PageCount { get; set; }
}
