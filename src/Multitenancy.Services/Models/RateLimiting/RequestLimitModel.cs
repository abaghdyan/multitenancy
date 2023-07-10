namespace Multitenancy.Services.Models.RateLimiting;

public class RequestLimitModel
{
    public int Quantity { get; set; }
    public bool AllowScale { get; set; }
    public DateTime CreatedDate { get; set; }
}
