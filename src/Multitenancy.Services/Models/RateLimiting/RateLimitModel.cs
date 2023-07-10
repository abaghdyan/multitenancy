namespace Multitenancy.Services.Models.RateLimiting;

public class RateLimitModel
{
    public int? RequestCount { get; set; }
    public int TimeWindowInSec { get; set; } = 1;
}
