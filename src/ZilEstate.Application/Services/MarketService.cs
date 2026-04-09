using Microsoft.EntityFrameworkCore;
using ZilEstate.Application.Common.Interfaces;
using ZilEstate.Application.DTOs;
using ZilEstate.Domain.Entities;
using ZilEstate.Domain.Enums;

namespace ZilEstate.Application.Services;

public class MarketService
{
    private readonly IApplicationDbContext _context;

    public MarketService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MarketOverviewDto> GetOverviewAsync(CancellationToken cancellationToken = default)
    {
        var properties = await _context.Properties
            .Include(p => p.Location)
            .Where(p => p.IsApproved)
            .ToListAsync(cancellationToken);

        if (!properties.Any())
            return new MarketOverviewDto();

        var forSale = properties.Where(p => p.Status == PropertyStatus.ForSale).ToList();
        var forRent = properties.Where(p => p.Status == PropertyStatus.ForRent).ToList();
        var withSize = properties.Where(p => p.SizeM2.HasValue && p.SizeM2 > 0).ToList();

        var districtStats = properties
            .GroupBy(p => p.Location.District)
            .Select(g =>
            {
                var sale = g.Where(p => p.Status == PropertyStatus.ForSale).ToList();
                var rent = g.Where(p => p.Status == PropertyStatus.ForRent).ToList();
                var sized = g.Where(p => p.SizeM2.HasValue && p.SizeM2 > 0).ToList();
                return new DistrictStatsDto
                {
                    District = g.Key,
                    ListingCount = g.Count(),
                    ForSaleCount = sale.Count,
                    ForRentCount = rent.Count,
                    AverageSalePrice = sale.Any() ? sale.Average(p => p.Price) : 0,
                    AverageRentPrice = rent.Any() ? rent.Average(p => p.Price) : 0,
                    AveragePricePerM2 = sized.Any()
                        ? (decimal)sized.Average(p => (double)(p.Price / (decimal)p.SizeM2!.Value))
                        : 0,
                };
            })
            .OrderByDescending(d => d.ListingCount)
            .ToList();

        var typeStats = properties
            .GroupBy(p => p.Type)
            .Select(g => new PropertyTypeStatsDto
            {
                Type = g.Key.ToString(),
                Count = g.Count(),
                AveragePrice = g.Average(p => p.Price),
                MinPrice = g.Min(p => p.Price),
                MaxPrice = g.Max(p => p.Price),
            })
            .OrderByDescending(t => t.Count)
            .ToList();

        return new MarketOverviewDto
        {
            TotalListings = properties.Count,
            ForSaleCount = forSale.Count,
            ForRentCount = forRent.Count,
            AverageSalePrice = forSale.Any() ? forSale.Average(p => p.Price) : 0,
            AverageRentPrice = forRent.Any() ? forRent.Average(p => p.Price) : 0,
            AveragePricePerM2 = withSize.Any()
                ? (decimal)withSize.Average(p => (double)(p.Price / (decimal)p.SizeM2!.Value))
                : 0,
            DistrictStats = districtStats,
            TypeStats = typeStats,
        };
    }

    public async Task<List<PriceTrendPointDto>> GetPriceTrendsAsync(int months = 12, string? district = null, CancellationToken cancellationToken = default)
    {
        var cutoff = DateTime.UtcNow.AddMonths(-months);
        var query = _context.Properties
            .Include(p => p.Location)
            .Where(p => p.IsApproved && p.CreatedAt >= cutoff);

        if (!string.IsNullOrEmpty(district))
            query = query.Where(p => p.Location.District == district);

        var properties = await query.ToListAsync(cancellationToken);

        return properties
            .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g =>
            {
                var sale = g.Where(p => p.Status == PropertyStatus.ForSale).ToList();
                var rent = g.Where(p => p.Status == PropertyStatus.ForRent).ToList();
                return new PriceTrendPointDto
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    AverageSalePrice = sale.Any() ? sale.Average(p => p.Price) : 0,
                    AverageRentPrice = rent.Any() ? rent.Average(p => p.Price) : 0,
                    ListingCount = g.Count()
                };
            })
            .ToList();
    }

    public async Task<DistrictDetailDto?> GetDistrictDetailAsync(string district, CancellationToken cancellationToken = default)
    {
        var properties = await _context.Properties
            .Include(p => p.Location)
            .Where(p => p.IsApproved && p.Location.District == district)
            .ToListAsync(cancellationToken);

        if (!properties.Any()) return null;

        var forSale = properties.Where(p => p.Status == PropertyStatus.ForSale).ToList();
        var forRent = properties.Where(p => p.Status == PropertyStatus.ForRent).ToList();
        var withSize = properties.Where(p => p.SizeM2.HasValue && p.SizeM2 > 0).ToList();

        var typeBreakdown = properties
            .GroupBy(p => p.Type)
            .Select(g => new PropertyTypeStatsDto
            {
                Type = g.Key.ToString(),
                Count = g.Count(),
                AveragePrice = g.Average(p => p.Price),
                MinPrice = g.Min(p => p.Price),
                MaxPrice = g.Max(p => p.Price),
            })
            .OrderByDescending(t => t.Count)
            .ToList();

        var trends = properties
            .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g =>
            {
                var sale = g.Where(p => p.Status == PropertyStatus.ForSale).ToList();
                var rent = g.Where(p => p.Status == PropertyStatus.ForRent).ToList();
                return new PriceTrendPointDto
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    AverageSalePrice = sale.Any() ? sale.Average(p => p.Price) : 0,
                    AverageRentPrice = rent.Any() ? rent.Average(p => p.Price) : 0,
                    ListingCount = g.Count()
                };
            })
            .ToList();

        return new DistrictDetailDto
        {
            District = district,
            TotalListings = properties.Count,
            ForSaleCount = forSale.Count,
            ForRentCount = forRent.Count,
            AverageSalePrice = forSale.Any() ? forSale.Average(p => p.Price) : 0,
            AverageRentPrice = forRent.Any() ? forRent.Average(p => p.Price) : 0,
            AveragePricePerM2 = withSize.Any()
                ? (decimal)withSize.Average(p => (double)(p.Price / (decimal)p.SizeM2!.Value))
                : 0,
            MinSalePrice = forSale.Any() ? forSale.Min(p => p.Price) : 0,
            MaxSalePrice = forSale.Any() ? forSale.Max(p => p.Price) : 0,
            TypeBreakdown = typeBreakdown,
            Trends = trends,
        };
    }

    public async Task<List<PriceBucketDto>> GetPriceDistributionAsync(string? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Properties.Where(p => p.IsApproved);

        bool isRent = status?.ToLower() == "forrent";
        if (!string.IsNullOrEmpty(status))
        {
            if (isRent)
                query = query.Where(p => p.Status == PropertyStatus.ForRent);
            else
                query = query.Where(p => p.Status == PropertyStatus.ForSale);
        }

        var prices = await query.Select(p => p.Price).ToListAsync(cancellationToken);

        if (isRent)
        {
            return new List<PriceBucketDto>
            {
                new() { Label = "< 10K", MinPrice = 0, MaxPrice = 10_000, Count = prices.Count(p => p < 10_000) },
                new() { Label = "10K–25K", MinPrice = 10_000, MaxPrice = 25_000, Count = prices.Count(p => p >= 10_000 && p < 25_000) },
                new() { Label = "25K–50K", MinPrice = 25_000, MaxPrice = 50_000, Count = prices.Count(p => p >= 25_000 && p < 50_000) },
                new() { Label = "50K–100K", MinPrice = 50_000, MaxPrice = 100_000, Count = prices.Count(p => p >= 50_000 && p < 100_000) },
                new() { Label = "> 100K", MinPrice = 100_000, MaxPrice = decimal.MaxValue, Count = prices.Count(p => p >= 100_000) },
            };
        }

        return new List<PriceBucketDto>
        {
            new() { Label = "< 1M", MinPrice = 0, MaxPrice = 1_000_000, Count = prices.Count(p => p < 1_000_000) },
            new() { Label = "1M–3M", MinPrice = 1_000_000, MaxPrice = 3_000_000, Count = prices.Count(p => p >= 1_000_000 && p < 3_000_000) },
            new() { Label = "3M–5M", MinPrice = 3_000_000, MaxPrice = 5_000_000, Count = prices.Count(p => p >= 3_000_000 && p < 5_000_000) },
            new() { Label = "5M–10M", MinPrice = 5_000_000, MaxPrice = 10_000_000, Count = prices.Count(p => p >= 5_000_000 && p < 10_000_000) },
            new() { Label = "10M–20M", MinPrice = 10_000_000, MaxPrice = 20_000_000, Count = prices.Count(p => p >= 10_000_000 && p < 20_000_000) },
            new() { Label = "> 20M", MinPrice = 20_000_000, MaxPrice = decimal.MaxValue, Count = prices.Count(p => p >= 20_000_000) },
        };
    }

    public async Task<MarketActivityDto> GetRecentActivityAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var weekStart = now.AddDays(-7);
        var twoWeeksStart = now.AddDays(-14);

        var properties = await _context.Properties
            .Where(p => p.IsApproved && p.CreatedAt >= twoWeeksStart)
            .ToListAsync(cancellationToken);

        var thisWeek = properties.Where(p => p.CreatedAt >= weekStart).ToList();
        var lastWeek = properties.Where(p => p.CreatedAt < weekStart).ToList();

        var thisWeekSale = thisWeek.Where(p => p.Status == PropertyStatus.ForSale).ToList();
        var lastWeekSale = lastWeek.Where(p => p.Status == PropertyStatus.ForSale).ToList();

        decimal weeklyChange = lastWeek.Count > 0
            ? Math.Round((decimal)(thisWeek.Count - lastWeek.Count) / lastWeek.Count * 100, 1)
            : (thisWeek.Count > 0 ? 100 : 0);

        decimal priceChange = lastWeekSale.Any() && thisWeekSale.Any()
            ? Math.Round((thisWeekSale.Average(p => p.Price) - lastWeekSale.Average(p => p.Price))
                / lastWeekSale.Average(p => p.Price) * 100, 1)
            : 0;

        return new MarketActivityDto
        {
            NewThisWeek = thisWeek.Count,
            NewLastWeek = lastWeek.Count,
            WeeklyChangePercent = weeklyChange,
            AverageSalePriceThisWeek = thisWeekSale.Any() ? thisWeekSale.Average(p => p.Price) : 0,
            AverageSalePriceLastWeek = lastWeekSale.Any() ? lastWeekSale.Average(p => p.Price) : 0,
            PriceChangePercent = priceChange,
            LastUpdated = now,
        };
    }

    public async Task<List<MostViewedPropertyDto>> GetMostViewedAsync(int count = 8, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .Include(p => p.Location)
            .Include(p => p.Images)
            .Where(p => p.IsApproved)
            .OrderByDescending(p => p.ViewCount)
            .Take(count)
            .Select(p => new MostViewedPropertyDto
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                Type = p.Type.ToString(),
                Status = p.Status.ToString(),
                District = p.Location.District,
                City = p.Location.City,
                ViewCount = p.ViewCount,
                PrimaryImageUrl = p.Images.Where(i => i.IsPrimary).Select(i => i.Url).FirstOrDefault()
                    ?? p.Images.Select(i => i.Url).FirstOrDefault(),
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PriceAlertDto> CreatePriceAlertAsync(int userId, CreatePriceAlertDto dto, CancellationToken cancellationToken = default)
    {
        var alert = new PriceAlert
        {
            UserId = userId,
            District = dto.District,
            PropertyType = dto.PropertyType,
            MaxPrice = dto.MaxPrice,
            Status = dto.Status,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };
        _context.PriceAlerts.Add(alert);
        await _context.SaveChangesAsync(cancellationToken);
        return MapAlert(alert);
    }

    public async Task<List<PriceAlertDto>> GetUserPriceAlertsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.PriceAlerts
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new PriceAlertDto
            {
                Id = a.Id,
                District = a.District,
                PropertyType = a.PropertyType,
                MaxPrice = a.MaxPrice,
                Status = a.Status,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt,
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> DeletePriceAlertAsync(int userId, int alertId, CancellationToken cancellationToken = default)
    {
        var alert = await _context.PriceAlerts.FirstOrDefaultAsync(a => a.Id == alertId && a.UserId == userId, cancellationToken);
        if (alert == null) return false;
        _context.PriceAlerts.Remove(alert);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static PriceAlertDto MapAlert(PriceAlert a) => new()
    {
        Id = a.Id,
        District = a.District,
        PropertyType = a.PropertyType,
        MaxPrice = a.MaxPrice,
        Status = a.Status,
        IsActive = a.IsActive,
        CreatedAt = a.CreatedAt,
    };

    public async Task<ValuationResultDto> GetPriceEstimateAsync(string district, string type, double? sizeM2, string status = "ForSale", CancellationToken cancellationToken = default)
    {
        var query = _context.Properties
            .Include(p => p.Location)
            .Where(p => p.IsApproved && p.Location.District == district);

        if (Enum.TryParse<ZilEstate.Domain.Enums.PropertyType>(type, true, out var propType))
            query = query.Where(p => p.Type == propType);

        if (Enum.TryParse<ZilEstate.Domain.Enums.PropertyStatus>(status, true, out var propStatus))
            query = query.Where(p => p.Status == propStatus);

        var comparables = await query.ToListAsync(cancellationToken);

        if (!comparables.Any())
        {
            return new ValuationResultDto { District = district, Type = type, ComparableCount = 0 };
        }

        var prices = comparables.Select(p => p.Price).OrderBy(x => x).ToList();
        var avg = prices.Average();
        var stdDev = Math.Sqrt(prices.Select(p => Math.Pow((double)(p - avg), 2)).Average());

        decimal avgPricePerM2 = 0;
        if (sizeM2.HasValue && sizeM2 > 0)
        {
            var withSize = comparables.Where(p => p.SizeM2.HasValue && p.SizeM2 > 0).ToList();
            if (withSize.Any())
                avgPricePerM2 = (decimal)withSize.Average(p => (double)(p.Price / (decimal)p.SizeM2!.Value));
        }

        var estimatedMid = sizeM2.HasValue && sizeM2 > 0 && avgPricePerM2 > 0
            ? avgPricePerM2 * (decimal)sizeM2.Value
            : avg;

        return new ValuationResultDto
        {
            EstimatedLow = Math.Round(estimatedMid * 0.88m, -3),
            EstimatedMid = Math.Round(estimatedMid, -3),
            EstimatedHigh = Math.Round(estimatedMid * 1.12m, -3),
            AvgPricePerM2 = Math.Round(avgPricePerM2, 0),
            ComparableCount = comparables.Count,
            District = district,
            Type = type,
        };
    }

    public async Task<List<PropertyListDto>> GetRecentlySoldAsync(int count = 6, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .Include(p => p.Location)
            .Include(p => p.Images)
            .Include(p => p.Agency)
            .Where(p => p.Status == ZilEstate.Domain.Enums.PropertyStatus.Sold || p.Status == ZilEstate.Domain.Enums.PropertyStatus.Rented)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .Select(p => new PropertyListDto
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                Type = p.Type.ToString(),
                Status = p.Status.ToString(),
                RentalType = p.RentalType.ToString(),
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                SizeM2 = p.SizeM2,
                District = p.Location.District,
                City = p.Location.City,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                PrimaryImageUrl = p.Images.Where(i => i.IsPrimary).Select(i => i.Url).FirstOrDefault()
                                  ?? p.Images.Select(i => i.Url).FirstOrDefault(),
                IsFeatured = p.IsFeatured,
                IsApproved = p.IsApproved,
                ViewCount = p.ViewCount,
                CreatedAt = p.CreatedAt,
            })
            .ToListAsync(cancellationToken);
    }
}
