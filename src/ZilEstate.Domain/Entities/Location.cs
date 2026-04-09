namespace ZilEstate.Domain.Entities;

public class Location
{
    public int Id { get; set; }
    public string District { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public ICollection<Property> Properties { get; set; } = new List<Property>();
}
