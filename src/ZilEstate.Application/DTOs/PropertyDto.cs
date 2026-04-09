namespace ZilEstate.Application.DTOs;

public class PropertyDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string RentalType { get; set; } = string.Empty;
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
    public int LocationId { get; set; }
    public string District { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? VideoUrl { get; set; }
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }
    public int? UserId { get; set; }
    public int? AgencyId { get; set; }
    public string? AgencyName { get; set; }
    public string? AgencyLogoUrl { get; set; }
    public bool SellerIsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal? PreviousPrice { get; set; }
    public bool IsPromoted { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsNew => (DateTime.UtcNow - CreatedAt).TotalDays <= 7;
    public bool IsPriceReduced => PreviousPrice.HasValue && PreviousPrice.Value > Price;
    public string? FloorPlanUrl { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public int InquiryCount { get; set; }
    public int ViewingRequestCount { get; set; }
    public List<string> ImageUrls { get; set; } = new();
}
