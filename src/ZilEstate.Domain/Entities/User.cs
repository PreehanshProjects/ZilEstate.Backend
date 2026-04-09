namespace ZilEstate.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // User or Admin
    public bool IsVerified { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? AgencyId { get; set; }
    public Agency? Agency { get; set; }

    public ICollection<Property> Properties { get; set; } = new List<Property>();
}
