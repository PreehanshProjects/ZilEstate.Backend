namespace ZilEstate.Domain.Entities;

public class CollectionItem
{
    public int Id { get; set; }
    public int CollectionId { get; set; }
    public Collection Collection { get; set; } = null!;
    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
