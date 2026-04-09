namespace ZilEstate.Application.DTOs;

public class TrackedPropertyDto
{
    public int PropertyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal TrackedPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal PriceDrop { get; set; }
    public double PriceDropPercent { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public DateTime TrackedAt { get; set; }
}
