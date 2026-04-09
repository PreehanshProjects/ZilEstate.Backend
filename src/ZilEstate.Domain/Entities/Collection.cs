namespace ZilEstate.Domain.Entities;

public class Collection
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<CollectionItem> Items { get; set; } = new List<CollectionItem>();
}
