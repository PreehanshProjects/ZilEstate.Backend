namespace ZilEstate.Domain.Entities;
public class SavedSearch
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? PropertyType { get; set; }
    public string? Status { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? Bedrooms { get; set; }
    public string? District { get; set; }
    public string? Keyword { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
