namespace ZilEstate.Application.DTOs;

public class MarketOverviewDto
{
    public int TotalListings { get; set; }
    public int ForSaleCount { get; set; }
    public int ForRentCount { get; set; }
    public decimal AverageSalePrice { get; set; }
    public decimal AverageRentPrice { get; set; }
    public decimal AveragePricePerM2 { get; set; }
    public List<DistrictStatsDto> DistrictStats { get; set; } = new();
    public List<PropertyTypeStatsDto> TypeStats { get; set; } = new();
}

public class DistrictStatsDto
{
    public string District { get; set; } = string.Empty;
    public int ListingCount { get; set; }
    public decimal AverageSalePrice { get; set; }
    public decimal AverageRentPrice { get; set; }
    public decimal AveragePricePerM2 { get; set; }
    public int ForSaleCount { get; set; }
    public int ForRentCount { get; set; }
}

public class PropertyTypeStatsDto
{
    public string Type { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
}

public class PriceTrendPointDto
{
    public string Month { get; set; } = string.Empty;
    public decimal AverageSalePrice { get; set; }
    public decimal AverageRentPrice { get; set; }
    public int ListingCount { get; set; }
}

public class DistrictDetailDto
{
    public string District { get; set; } = string.Empty;
    public int TotalListings { get; set; }
    public int ForSaleCount { get; set; }
    public int ForRentCount { get; set; }
    public decimal AverageSalePrice { get; set; }
    public decimal AverageRentPrice { get; set; }
    public decimal AveragePricePerM2 { get; set; }
    public decimal MinSalePrice { get; set; }
    public decimal MaxSalePrice { get; set; }
    public List<PropertyTypeStatsDto> TypeBreakdown { get; set; } = new();
    public List<PriceTrendPointDto> Trends { get; set; } = new();
}

public class PriceBucketDto
{
    public string Label { get; set; } = string.Empty;
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public int Count { get; set; }
}

public class MarketActivityDto
{
    public int NewThisWeek { get; set; }
    public int NewLastWeek { get; set; }
    public decimal WeeklyChangePercent { get; set; }
    public decimal AverageSalePriceThisWeek { get; set; }
    public decimal AverageSalePriceLastWeek { get; set; }
    public decimal PriceChangePercent { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class MostViewedPropertyDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public string? PrimaryImageUrl { get; set; }
}

public class PriceAlertDto
{
    public int Id { get; set; }
    public string? District { get; set; }
    public string? PropertyType { get; set; }
    public decimal MaxPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePriceAlertDto
{
    public string? District { get; set; }
    public string? PropertyType { get; set; }
    public decimal MaxPrice { get; set; }
    public string Status { get; set; } = "ForSale";
}
