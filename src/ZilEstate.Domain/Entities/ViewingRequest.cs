namespace ZilEstate.Domain.Entities;
public class ViewingRequest
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
    public int? UserId { get; set; }
    public User? User { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public DateTime PreferredDate { get; set; }
    public string? Message { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Declined
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
