using System.ComponentModel.DataAnnotations;
using ZilEstate.Domain.Enums;

namespace ZilEstate.Application.DTOs;

public class CreatePropertyDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(5000)]
    public string Description { get; set; } = string.Empty;

    [Required, Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    public PropertyType Type { get; set; }

    [Required]
    public PropertyStatus Status { get; set; }

    public RentalType RentalType { get; set; } = RentalType.None;
    public decimal? PricePerNight { get; set; }
    public decimal? PricePerWeek { get; set; }
    public int? MinimumStayDays { get; set; }

    [Range(0, 50)]
    public int? Bedrooms { get; set; }

    [Range(0, 50)]
    public int? Bathrooms { get; set; }

    [Range(0, 50)]
    public int? Parking { get; set; }

    public double? SizeM2 { get; set; }
    public double? SizeArpents { get; set; }
    public bool? Furnished { get; set; }

    [Required, MaxLength(100)]
    public string SellerName { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string SellerPhone { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? SellerWhatsApp { get; set; }

    [EmailAddress, MaxLength(200)]
    public string? SellerEmail { get; set; }

    [Required]
    public int LocationId { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    [MaxLength(500)]
    public string? VideoUrl { get; set; }

    public List<string> ImageUrls { get; set; } = new();
}
