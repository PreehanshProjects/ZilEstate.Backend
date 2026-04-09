using ZilEstate.Domain.Enums;

namespace ZilEstate.Domain.Entities;

public class Property
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public PropertyType Type { get; set; }
    public PropertyStatus Status { get; set; }
    public RentalType RentalType { get; set; } = RentalType.None;
    public decimal? PricePerNight { get; set; }
    public decimal? PricePerWeek { get; set; }
    public int? MinimumStayDays { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public int? Parking { get; set; }
    public double? SizeM2 { get; set; }
    public double? SizeArpents { get; set; }
    public bool? Furnished { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public string SellerPhone { get; set; } = string.Empty;
    public string? SellerWhatsApp { get; set; }
    public string? SellerEmail { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public int? AgencyId { get; set; }
    public Agency? Agency { get; set; }
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? VideoUrl { get; set; }
    public bool IsApproved { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public int ViewCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal? PreviousPrice { get; set; }
    public DateTime? PriceChangedAt { get; set; }
    public bool IsPromoted { get; set; } = false;
    public DateTime? PromotedUntil { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? FloorPlanUrl { get; set; }
    public bool IsVerified { get; set; } = false;
    public DateTime? VerifiedAt { get; set; }
    public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<PropertyQuestion> Questions { get; set; } = new List<PropertyQuestion>();
    public ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();
}
