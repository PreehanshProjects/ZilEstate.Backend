namespace ZilEstate.Application.DTOs;

public class PriceHistoryDto
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public string? Note { get; set; }
    public DateTime ChangedAt { get; set; }
}
