using Microsoft.EntityFrameworkCore;
using ZilEstate.Application.Common.Interfaces;
using ZilEstate.Application.Common.Models;
using ZilEstate.Application.DTOs;
using ZilEstate.Domain.Entities;
using ZilEstate.Domain.Enums;

namespace ZilEstate.Application.Services;

public class PropertyService
{
    private readonly IApplicationDbContext _context;

    public PropertyService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<PropertyListDto>> GetPropertiesAsync(
        int page = 1,
        int pageSize = 12,
        string? type = null,
        string? status = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int? bedrooms = null,
        int? bathrooms = null,
        int? locationId = null,
        string? keyword = null,
        string sortBy = "newest",
        bool includeUnapproved = false,
        string? rentalType = null,
        string? district = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Properties
            .Include(p => p.Location)
            .Include(p => p.Images)
            .Include(p => p.Agency)
            .AsQueryable();

        if (!includeUnapproved)
            query = query.Where(p => p.IsApproved);

        if (!string.IsNullOrEmpty(type) && Enum.TryParse<PropertyType>(type, true, out var propType))
            query = query.Where(p => p.Type == propType);

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<PropertyStatus>(status, true, out var propStatus))
            query = query.Where(p => p.Status == propStatus);

        if (!string.IsNullOrEmpty(rentalType) && Enum.TryParse<RentalType>(rentalType, true, out var rt))
            query = query.Where(p => p.RentalType == rt);

        if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);
        if (bedrooms.HasValue) query = query.Where(p => p.Bedrooms >= bedrooms.Value);
        if (bathrooms.HasValue) query = query.Where(p => p.Bathrooms >= bathrooms.Value);
        if (locationId.HasValue) query = query.Where(p => p.LocationId == locationId.Value);

        if (!string.IsNullOrEmpty(district))
            query = query.Where(p => p.Location.District == district || p.Location.City == district);

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(p =>
                p.Title.Contains(keyword) ||
                p.Description.Contains(keyword) ||
                p.Location.City.Contains(keyword) ||
                p.Location.District.Contains(keyword));

        query = sortBy switch
        {
            "price_asc"  => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            _            => query.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PropertyListDto
            {
                Id              = p.Id,
                Title           = p.Title,
                Price           = p.Price,
                Type            = p.Type.ToString(),
                Status          = p.Status.ToString(),
                RentalType      = p.RentalType.ToString(),
                PricePerNight   = p.PricePerNight,
                PricePerWeek    = p.PricePerWeek,
                MinimumStayDays = p.MinimumStayDays,
                Bedrooms        = p.Bedrooms,
                Bathrooms       = p.Bathrooms,
                SizeM2          = p.SizeM2,
                District        = p.Location.District,
                City            = p.Location.City,
                Latitude        = p.Latitude,
                Longitude       = p.Longitude,
                PrimaryImageUrl = p.Images.Where(i => i.IsPrimary).Select(i => i.Url).FirstOrDefault()
                                  ?? p.Images.Select(i => i.Url).FirstOrDefault(),
                IsFeatured      = p.IsFeatured,
                IsApproved      = p.IsApproved,
                ViewCount       = p.ViewCount,
                UserId          = p.UserId,
                AgencyId        = p.AgencyId,
                AgencyName      = p.Agency != null ? p.Agency.Name : null,
                AgencyLogoUrl   = p.Agency != null ? p.Agency.LogoUrl : null,
                CreatedAt       = p.CreatedAt,
                PreviousPrice   = p.PreviousPrice,
                IsPromoted      = p.IsPromoted,
                ExpiresAt       = p.ExpiresAt,
                FloorPlanUrl    = p.FloorPlanUrl,
                IsVerified      = p.IsVerified
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<PropertyListDto>
        {
            Items      = items,
            TotalCount = totalCount,
            Page       = page,
            PageSize   = pageSize
        };
    }

    public async Task<PropertyDto?> GetPropertyByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var p = await _context.Properties
            .Include(p => p.Location)
            .Include(p => p.Images)
            .Include(p => p.User)
            .Include(p => p.Agency)
            .Where(p => p.Id == id && p.IsApproved)
            .FirstOrDefaultAsync(cancellationToken);

        if (p == null) return null;

        // Increment view count
        p.ViewCount++;
        await _context.SaveChangesAsync(cancellationToken);

        var dto = MapToDto(p);
        dto.InquiryCount = await _context.Inquiries.CountAsync(i => i.PropertyId == id, cancellationToken);
        dto.ViewingRequestCount = await _context.ViewingRequests.CountAsync(v => v.PropertyId == id, cancellationToken);
        return dto;
    }

    // Allows owner to view their own unapproved property
    public async Task<PropertyDto?> GetPropertyByIdForOwnerAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var p = await _context.Properties
            .Include(p => p.Location)
            .Include(p => p.Images)
            .Include(p => p.User)
            .Include(p => p.Agency)
            .Where(p => p.Id == id && (p.IsApproved || p.UserId == userId))
            .FirstOrDefaultAsync(cancellationToken);

        if (p == null) return null;

        var dto = MapToDto(p);
        dto.InquiryCount = await _context.Inquiries.CountAsync(i => i.PropertyId == id, cancellationToken);
        dto.ViewingRequestCount = await _context.ViewingRequests.CountAsync(v => v.PropertyId == id, cancellationToken);
        return dto;
    }

    private static PropertyDto MapToDto(Property p) => new()
    {
        Id             = p.Id,
        Title          = p.Title,
        Description    = p.Description,
        Price          = p.Price,
        Type           = p.Type.ToString(),
        Status         = p.Status.ToString(),
        RentalType     = p.RentalType.ToString(),
        PricePerNight  = p.PricePerNight,
        PricePerWeek   = p.PricePerWeek,
        MinimumStayDays = p.MinimumStayDays,
        Bedrooms       = p.Bedrooms,
        Bathrooms      = p.Bathrooms,
        Parking        = p.Parking,
        SizeM2         = p.SizeM2,
        SizeArpents    = p.SizeArpents,
        Furnished      = p.Furnished,
        SellerName     = p.SellerName,
        SellerPhone    = p.SellerPhone,
        SellerWhatsApp = p.SellerWhatsApp,
        SellerEmail    = p.SellerEmail,
        LocationId     = p.LocationId,
        District       = p.Location.District,
        City           = p.Location.City,
        Latitude       = p.Latitude,
        Longitude      = p.Longitude,
        VideoUrl       = p.VideoUrl,
        IsFeatured     = p.IsFeatured,
        ViewCount      = p.ViewCount,
        UserId         = p.UserId,
        AgencyId       = p.AgencyId,
        AgencyName     = p.Agency?.Name,
        AgencyLogoUrl  = p.Agency?.LogoUrl,
        SellerIsVerified = p.User?.IsVerified ?? p.Agency?.IsVerified ?? false,
        CreatedAt      = p.CreatedAt,
        PreviousPrice  = p.PreviousPrice,
        IsPromoted     = p.IsPromoted,
        ExpiresAt      = p.ExpiresAt,
        FloorPlanUrl   = p.FloorPlanUrl,
        IsVerified     = p.IsVerified,
        VerifiedAt     = p.VerifiedAt,
        ImageUrls      = p.Images.OrderByDescending(i => i.IsPrimary).Select(i => i.Url).ToList()
    };

    public async Task<List<PropertyListDto>> GetFeaturedPropertiesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .Include(p => p.Location)
            .Include(p => p.Images)
            .Include(p => p.Agency)
            .Where(p => p.IsApproved && p.IsFeatured)
            .OrderByDescending(p => p.CreatedAt)
            .Take(6)
            .Select(p => new PropertyListDto
            {
                Id              = p.Id,
                Title           = p.Title,
                Price           = p.Price,
                Type            = p.Type.ToString(),
                Status          = p.Status.ToString(),
                RentalType      = p.RentalType.ToString(),
                PricePerNight   = p.PricePerNight,
                PricePerWeek    = p.PricePerWeek,
                MinimumStayDays = p.MinimumStayDays,
                Bedrooms        = p.Bedrooms,
                Bathrooms       = p.Bathrooms,
                SizeM2          = p.SizeM2,
                District        = p.Location.District,
                City            = p.Location.City,
                Latitude        = p.Latitude,
                Longitude       = p.Longitude,
                PrimaryImageUrl = p.Images.Where(i => i.IsPrimary).Select(i => i.Url).FirstOrDefault()
                                  ?? p.Images.Select(i => i.Url).FirstOrDefault(),
                IsFeatured      = p.IsFeatured,
                ViewCount       = p.ViewCount,
                AgencyId        = p.AgencyId,
                AgencyName      = p.Agency != null ? p.Agency.Name : null,
                AgencyLogoUrl   = p.Agency != null ? p.Agency.LogoUrl : null,
                CreatedAt       = p.CreatedAt,
                PreviousPrice   = p.PreviousPrice,
                IsPromoted      = p.IsPromoted,
                ExpiresAt       = p.ExpiresAt,
                FloorPlanUrl    = p.FloorPlanUrl,
                IsVerified      = p.IsVerified
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PropertyListDto>> GetMyPropertiesAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .Include(p => p.Location)
            .Include(p => p.Images)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PropertyListDto
            {
                Id              = p.Id,
                Title           = p.Title,
                Price           = p.Price,
                Type            = p.Type.ToString(),
                Status          = p.Status.ToString(),
                RentalType      = p.RentalType.ToString(),
                PricePerNight   = p.PricePerNight,
                PricePerWeek    = p.PricePerWeek,
                MinimumStayDays = p.MinimumStayDays,
                Bedrooms        = p.Bedrooms,
                Bathrooms       = p.Bathrooms,
                SizeM2          = p.SizeM2,
                District        = p.Location.District,
                City            = p.Location.City,
                Latitude        = p.Latitude,
                Longitude       = p.Longitude,
                PrimaryImageUrl = p.Images.Where(i => i.IsPrimary).Select(i => i.Url).FirstOrDefault()
                                  ?? p.Images.Select(i => i.Url).FirstOrDefault(),
                IsFeatured      = p.IsFeatured,
                IsApproved      = p.IsApproved,
                ViewCount       = p.ViewCount,
                UserId          = p.UserId,
                AgencyId        = p.AgencyId,
                AgencyName      = p.Agency != null ? p.Agency.Name : null,
                AgencyLogoUrl   = p.Agency != null ? p.Agency.LogoUrl : null,
                CreatedAt       = p.CreatedAt,
                PreviousPrice   = p.PreviousPrice,
                IsPromoted      = p.IsPromoted,
                ExpiresAt       = p.ExpiresAt,
                FloorPlanUrl    = p.FloorPlanUrl,
                IsVerified      = p.IsVerified
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto dto, int? userId = null, CancellationToken cancellationToken = default)
    {
        int? agencyId = null;
        if (userId.HasValue)
        {
            var user = await _context.Users.FindAsync(new object[] { userId.Value }, cancellationToken);
            agencyId = user?.AgencyId;
        }

        var property = new Property
        {
            Title          = dto.Title,
            Description    = dto.Description,
            Price          = dto.Price,
            Type           = dto.Type,
            Status         = dto.Status,
            RentalType     = dto.RentalType,
            PricePerNight  = dto.PricePerNight,
            PricePerWeek   = dto.PricePerWeek,
            MinimumStayDays = dto.MinimumStayDays,
            Bedrooms       = dto.Bedrooms,
            Bathrooms      = dto.Bathrooms,
            Parking        = dto.Parking,
            SizeM2         = dto.SizeM2,
            SizeArpents    = dto.SizeArpents,
            Furnished      = dto.Furnished,
            SellerName     = dto.SellerName,
            SellerPhone    = dto.SellerPhone,
            SellerWhatsApp = dto.SellerWhatsApp,
            SellerEmail    = dto.SellerEmail,
            LocationId     = dto.LocationId,
            Latitude       = dto.Latitude,
            Longitude      = dto.Longitude,
            VideoUrl       = dto.VideoUrl,
            UserId         = userId,
            AgencyId       = agencyId,
            IsApproved     = false,
            ExpiresAt      = DateTime.UtcNow.AddDays(90),
            Images         = dto.ImageUrls.Select((url, idx) => new PropertyImage
            {
                Url       = url,
                IsPrimary = idx == 0
            }).ToList()
        };

        _context.Properties.Add(property);
        await _context.SaveChangesAsync(cancellationToken);

        // Return without incrementing view count
        var created = await _context.Properties
            .Include(p => p.Location)
            .Include(p => p.Images)
            .Include(p => p.User)
            .Include(p => p.Agency)
            .FirstAsync(p => p.Id == property.Id, cancellationToken);

        return MapToDto(created);
    }

    public async Task<PropertyDto?> UpdatePropertyAsync(int id, CreatePropertyDto dto, int userId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var property = await _context.Properties
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (property == null || (!isAdmin && property.UserId != userId))
            return null;

        property.Title         = dto.Title;
        property.Description   = dto.Description;
        // Track price change
        if (property.Price != dto.Price)
        {
            _context.PriceHistories.Add(new ZilEstate.Domain.Entities.PriceHistory
            {
                PropertyId = property.Id,
                Price = property.Price,
                Note = "Price updated",
                ChangedAt = DateTime.UtcNow,
            });
            property.PreviousPrice = property.Price;
            property.PriceChangedAt = DateTime.UtcNow;
        }
        property.Price         = dto.Price;
        property.Type          = dto.Type;
        property.Status        = dto.Status;
        property.Bedrooms      = dto.Bedrooms;
        property.Bathrooms     = dto.Bathrooms;
        property.Parking       = dto.Parking;
        property.SizeM2        = dto.SizeM2;
        property.SizeArpents   = dto.SizeArpents;
        property.Furnished     = dto.Furnished;
        property.SellerName    = dto.SellerName;
        property.SellerPhone   = dto.SellerPhone;
        property.SellerWhatsApp = dto.SellerWhatsApp;
        property.SellerEmail   = dto.SellerEmail;
        property.LocationId    = dto.LocationId;
        property.Latitude      = dto.Latitude;
        property.Longitude     = dto.Longitude;
        property.VideoUrl      = dto.VideoUrl;

        _context.PropertyImages.RemoveRange(property.Images);
        property.Images = dto.ImageUrls.Select((url, idx) => new PropertyImage
        {
            Url       = url,
            IsPrimary = idx == 0
        }).ToList();

        await _context.SaveChangesAsync(cancellationToken);
        return await GetPropertyByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeletePropertyAsync(int id, int userId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var property = await _context.Properties.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (property == null || (!isAdmin && property.UserId != userId))
            return false;

        _context.Properties.Remove(property);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> CloseListingAsync(int id, int userId, string statusStr, CancellationToken cancellationToken = default)
    {
        var property = await _context.Properties.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (property == null || property.UserId != userId)
            return false;

        if (!Enum.TryParse<PropertyStatus>(statusStr, true, out var newStatus)
            || (newStatus != PropertyStatus.Sold && newStatus != PropertyStatus.Rented))
            return false;

        property.Status = newStatus;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ToggleApprovalAsync(int id, CancellationToken cancellationToken = default)
    {
        var p = await _context.Properties.FindAsync(new object[] { id }, cancellationToken);
        if (p == null) return false;
        p.IsApproved = !p.IsApproved;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ToggleFeaturedAsync(int id, CancellationToken cancellationToken = default)
    {
        var p = await _context.Properties.FindAsync(new object[] { id }, cancellationToken);
        if (p == null) return false;
        p.IsFeatured = !p.IsFeatured;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<List<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Select(u => new UserDto
            {
                Id           = u.Id,
                FullName     = u.FullName,
                Email        = u.Email,
                Role         = u.Role,
                IsVerified   = u.IsVerified,
                ListingCount = u.Properties.Count(),
                CreatedAt    = u.CreatedAt
            })
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ToggleUserVerifiedAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null) return false;
        user.IsVerified = !user.IsVerified;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SetUserRoleAsync(int userId, string role, CancellationToken cancellationToken = default)
    {
        if (role != "Admin" && role != "User") return false;
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null) return false;
        user.Role = role;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<decimal> EstimatePriceAsync(
        int locationId, double? sizeM2,
        PropertyType type, PropertyStatus status,
        CancellationToken cancellationToken = default)
    {
        var comparables = await _context.Properties
            .Where(p => p.IsApproved && p.LocationId == locationId && p.Type == type && p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        if (!comparables.Any())
        {
            return status == PropertyStatus.ForRent
                ? type == PropertyType.Land ? 0 : 25000m
                : type == PropertyType.Land ? 2_000_000m : 5_000_000m;
        }

        if (sizeM2.HasValue && comparables.All(c => c.SizeM2.HasValue))
        {
            var avgPerM2 = comparables.Average(c => (double)(c.Price / (decimal)c.SizeM2!.Value));
            return (decimal)(avgPerM2 * sizeM2.Value);
        }

        return comparables.Average(c => c.Price);
    }

    public async Task<List<PropertyListDto>> GetSimilarAsync(int propertyId, int count = 4, CancellationToken cancellationToken = default)
    {
        var property = await _context.Properties.Include(p => p.Location).FirstOrDefaultAsync(p => p.Id == propertyId, cancellationToken);
        if (property == null) return new();
        return await _context.Properties
            .Include(p => p.Location).Include(p => p.Images).Include(p => p.Agency)
            .Where(p => p.IsApproved && p.Id != propertyId && p.Type == property.Type && p.LocationId == property.LocationId)
            .OrderBy(p => Math.Abs((double)(p.Price - property.Price)))
            .Take(count)
            .Select(p => new PropertyListDto
            {
                Id = p.Id, Title = p.Title, Price = p.Price, Type = p.Type.ToString(), Status = p.Status.ToString(),
                RentalType = p.RentalType.ToString(), Bedrooms = p.Bedrooms, Bathrooms = p.Bathrooms, SizeM2 = p.SizeM2,
                District = p.Location.District, City = p.Location.City, Latitude = p.Latitude, Longitude = p.Longitude,
                PrimaryImageUrl = p.Images.Where(i => i.IsPrimary).Select(i => i.Url).FirstOrDefault() ?? p.Images.Select(i => i.Url).FirstOrDefault(),
                IsFeatured = p.IsFeatured, IsApproved = p.IsApproved, ViewCount = p.ViewCount,
                UserId = p.UserId, AgencyId = p.AgencyId,
                AgencyName = p.Agency != null ? p.Agency.Name : null, AgencyLogoUrl = p.Agency != null ? p.Agency.LogoUrl : null,
                CreatedAt = p.CreatedAt, PreviousPrice = p.PreviousPrice, IsPromoted = p.IsPromoted, ExpiresAt = p.ExpiresAt, FloorPlanUrl = p.FloorPlanUrl,
                IsVerified = p.IsVerified
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PropertyAnalyticsDto?> GetAnalyticsAsync(int propertyId, int userId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var p = await _context.Properties
            .Include(p => p.Reviews).Include(p => p.Questions)
            .FirstOrDefaultAsync(x => x.Id == propertyId, cancellationToken);
        if (p == null || (!isAdmin && p.UserId != userId)) return null;
        var inquiryCount = await _context.Inquiries.CountAsync(i => i.PropertyId == propertyId, cancellationToken);
        var viewingCount = await _context.ViewingRequests.CountAsync(v => v.PropertyId == propertyId, cancellationToken);
        var avgRating = p.Reviews.Any() ? (decimal?)p.Reviews.Average(r => r.Rating) : null;
        return new PropertyAnalyticsDto { PropertyId = p.Id, Title = p.Title, ViewCount = p.ViewCount, InquiryCount = inquiryCount, ViewingRequestCount = viewingCount, ReviewCount = p.Reviews.Count, QuestionCount = p.Questions.Count, AverageRating = avgRating };
    }

    public async Task<bool> TogglePromotedAsync(int id, CancellationToken cancellationToken = default)
    {
        var p = await _context.Properties.FindAsync(new object[] { id }, cancellationToken);
        if (p == null) return false;
        p.IsPromoted = !p.IsPromoted;
        p.PromotedUntil = p.IsPromoted ? DateTime.UtcNow.AddDays(30) : null;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RenewListingAsync(int id, int userId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var p = await _context.Properties.FindAsync(new object[] { id }, cancellationToken);
        if (p == null || (!isAdmin && p.UserId != userId)) return false;
        p.ExpiresAt = DateTime.UtcNow.AddDays(90);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> BulkApproveAsync(List<int> ids, CancellationToken cancellationToken = default)
    {
        var props = await _context.Properties.Where(p => ids.Contains(p.Id)).ToListAsync(cancellationToken);
        foreach (var p in props) p.IsApproved = true;
        await _context.SaveChangesAsync(cancellationToken);
        return props.Count;
    }

    public async Task<int> BulkRejectAsync(List<int> ids, CancellationToken cancellationToken = default)
    {
        var props = await _context.Properties.Where(p => ids.Contains(p.Id)).ToListAsync(cancellationToken);
        foreach (var p in props) p.IsApproved = false;
        await _context.SaveChangesAsync(cancellationToken);
        return props.Count;
    }

    public async Task<int> BulkFeatureAsync(List<int> ids, bool feature, CancellationToken cancellationToken = default)
    {
        var props = await _context.Properties.Where(p => ids.Contains(p.Id)).ToListAsync(cancellationToken);
        foreach (var p in props) p.IsFeatured = feature;
        await _context.SaveChangesAsync(cancellationToken);
        return props.Count;
    }

    public async Task<List<PriceHistoryDto>> GetPriceHistoryAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.PriceHistories
            .Where(h => h.PropertyId == id)
            .OrderByDescending(h => h.ChangedAt)
            .Select(h => new PriceHistoryDto { Id = h.Id, Price = h.Price, Note = h.Note, ChangedAt = h.ChangedAt })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> VerifyPropertyAsync(int id, CancellationToken cancellationToken = default)
    {
        var p = await _context.Properties.FindAsync(new object[] { id }, cancellationToken);
        if (p == null) return false;
        p.IsVerified = !p.IsVerified;
        p.VerifiedAt = p.IsVerified ? DateTime.UtcNow : null;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ListingScoreDto> GetListingScoreAsync(int id, CancellationToken cancellationToken = default)
    {
        var p = await _context.Properties
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (p == null) return new ListingScoreDto();

        var items = new List<ScoreItem>
        {
            new() { Label = "Has title (50+ chars)", Points = 10, Achieved = p.Title.Length >= 50 },
            new() { Label = "Has description (200+ chars)", Points = 15, Achieved = p.Description.Length >= 200 },
            new() { Label = "Has at least 3 photos", Points = 20, Achieved = p.Images.Count >= 3 },
            new() { Label = "Has 6+ photos", Points = 10, Achieved = p.Images.Count >= 6 },
            new() { Label = "Has floor plan", Points = 10, Achieved = p.FloorPlanUrl != null },
            new() { Label = "Has video tour", Points = 10, Achieved = p.VideoUrl != null },
            new() { Label = "Has WhatsApp contact", Points = 5, Achieved = p.SellerWhatsApp != null },
            new() { Label = "Has email contact", Points = 5, Achieved = p.SellerEmail != null },
            new() { Label = "Specifies size (m²)", Points = 5, Achieved = p.SizeM2.HasValue },
            new() { Label = "Specifies bedrooms & bathrooms", Points = 5, Achieved = p.Bedrooms.HasValue && p.Bathrooms.HasValue },
            new() { Label = "Specifies furnished status", Points = 5, Achieved = p.Furnished.HasValue },
        };

        var score = items.Where(i => i.Achieved).Sum(i => i.Points);
        var maxScore = items.Sum(i => i.Points);
        var grade = score >= 80 ? "A" : score >= 60 ? "B" : score >= 40 ? "C" : "D";

        return new ListingScoreDto { Score = score, MaxScore = maxScore, Grade = grade, Items = items };
    }

    // Property Tracking (Feature 5)
    public async Task<bool> TrackPropertyAsync(int propertyId, int userId, CancellationToken cancellationToken = default)
    {
        var existing = await _context.PropertyTrackings
            .FirstOrDefaultAsync(t => t.PropertyId == propertyId && t.UserId == userId, cancellationToken);
        if (existing != null) return true; // already tracked

        var property = await _context.Properties.FindAsync(new object[] { propertyId }, cancellationToken);
        if (property == null) return false;

        _context.PropertyTrackings.Add(new Domain.Entities.PropertyTracking
        {
            UserId = userId,
            PropertyId = propertyId,
            TrackedPrice = property.Price,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UntrackPropertyAsync(int propertyId, int userId, CancellationToken cancellationToken = default)
    {
        var tracking = await _context.PropertyTrackings
            .FirstOrDefaultAsync(t => t.PropertyId == propertyId && t.UserId == userId, cancellationToken);
        if (tracking == null) return false;

        _context.PropertyTrackings.Remove(tracking);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<List<TrackedPropertyDto>> GetTrackedPropertiesAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyTrackings
            .Include(t => t.Property)
                .ThenInclude(p => p.Images)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TrackedPropertyDto
            {
                PropertyId = t.PropertyId,
                Title = t.Property.Title,
                TrackedPrice = t.TrackedPrice,
                CurrentPrice = t.Property.Price,
                PriceDrop = t.TrackedPrice > t.Property.Price ? t.TrackedPrice - t.Property.Price : 0,
                PriceDropPercent = t.TrackedPrice > 0 && t.TrackedPrice > t.Property.Price
                    ? Math.Round((double)((t.TrackedPrice - t.Property.Price) / t.TrackedPrice * 100), 2)
                    : 0,
                PrimaryImageUrl = t.Property.Images.Where(i => i.IsPrimary).Select(i => i.Url).FirstOrDefault()
                                  ?? t.Property.Images.Select(i => i.Url).FirstOrDefault(),
                TrackedAt = t.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
