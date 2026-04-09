namespace ZilEstate.Domain.Entities;
public class AgencyReview
{
    public int Id { get; set; }
    public int AgencyId { get; set; }
    public Agency Agency { get; set; } = null!;
    public string AuthorName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
