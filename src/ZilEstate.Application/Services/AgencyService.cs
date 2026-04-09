using Microsoft.EntityFrameworkCore;
using ZilEstate.Application.Common.Interfaces;
using ZilEstate.Application.DTOs;
using ZilEstate.Domain.Entities;

namespace ZilEstate.Application.Services;

public class AgencyService
{
    private readonly IApplicationDbContext _context;

    public AgencyService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AgencyDto>> GetAllAgenciesAsync()
    {
        return await _context.Agencies
            .Select(a => new AgencyDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                LogoUrl = a.LogoUrl,
                Website = a.Website,
                Phone = a.Phone,
                Email = a.Email,
                Address = a.Address,
                IsVerified = a.IsVerified,
                PropertyCount = a.Properties.Count,
                Plan = a.Plan,
                ReviewCount = a.Reviews.Count,
                AverageRating = a.Reviews.Any() ? (decimal)a.Reviews.Average(r => r.Rating) : 0,
                AverageResponseHours = a.AverageResponseHours
            })
            .ToListAsync();
    }

    public async Task<AgencyDetailDto?> GetAgencyByIdAsync(int id)
    {
        var agency = await _context.Agencies
            .Include(a => a.Properties)
                .ThenInclude(p => p.Images)
            .Include(a => a.Properties)
                .ThenInclude(p => p.Location)
            .Include(a => a.Reviews)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (agency == null) return null;

        return new AgencyDetailDto
        {
            Id = agency.Id,
            Name = agency.Name,
            Description = agency.Description,
            LogoUrl = agency.LogoUrl,
            Website = agency.Website,
            Phone = agency.Phone,
            Email = agency.Email,
            Address = agency.Address,
            IsVerified = agency.IsVerified,
            PropertyCount = agency.Properties.Count,
            Plan = agency.Plan,
            ReviewCount = agency.Reviews.Count,
            AverageRating = agency.Reviews.Any() ? (decimal)agency.Reviews.Average(r => r.Rating) : 0,
            AverageResponseHours = agency.AverageResponseHours,
            Properties = agency.Properties.Select(p => new PropertyListDto
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                Type = p.Type.ToString(),
                Status = p.Status.ToString(),
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                SizeM2 = p.SizeM2,
                District = p.Location.District,
                City = p.Location.City,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                PrimaryImageUrl = p.Images.FirstOrDefault(i => i.IsPrimary)?.Url ?? p.Images.FirstOrDefault()?.Url,
                IsFeatured = p.IsFeatured,
                IsApproved = p.IsApproved,
                ViewCount = p.ViewCount,
                UserId = p.UserId,
                CreatedAt = p.CreatedAt
            }).ToList()
        };
    }

    public async Task<AgencyDto> CreateAgencyAsync(CreateAgencyDto dto)
    {
        var agency = new Agency
        {
            Name = dto.Name,
            Description = dto.Description,
            LogoUrl = dto.LogoUrl,
            Website = dto.Website,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            IsVerified = false
        };

        _context.Agencies.Add(agency);
        await _context.SaveChangesAsync();

        return new AgencyDto
        {
            Id = agency.Id,
            Name = agency.Name,
            Description = agency.Description,
            LogoUrl = agency.LogoUrl,
            Website = agency.Website,
            Phone = agency.Phone,
            Email = agency.Email,
            Address = agency.Address,
            IsVerified = agency.IsVerified,
            PropertyCount = 0
        };
    }

    public async Task<AgencyDto?> UpdateAgencyAsync(int id, CreateAgencyDto dto)
    {
        var agency = await _context.Agencies.FindAsync(id);
        if (agency == null) return null;

        agency.Name = dto.Name;
        agency.Description = dto.Description;
        agency.LogoUrl = dto.LogoUrl;
        agency.Website = dto.Website;
        agency.Phone = dto.Phone;
        agency.Email = dto.Email;
        agency.Address = dto.Address;

        await _context.SaveChangesAsync();

        return new AgencyDto
        {
            Id = agency.Id,
            Name = agency.Name,
            Description = agency.Description,
            LogoUrl = agency.LogoUrl,
            Website = agency.Website,
            Phone = agency.Phone,
            Email = agency.Email,
            Address = agency.Address,
            IsVerified = agency.IsVerified
        };
    }

    public async Task<List<AgencyReviewDto>> GetReviewsAsync(int agencyId)
    {
        return await _context.AgencyReviews
            .Where(r => r.AgencyId == agencyId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new AgencyReviewDto { Id = r.Id, AuthorName = r.AuthorName, Rating = r.Rating, Comment = r.Comment, CreatedAt = r.CreatedAt })
            .ToListAsync();
    }

    public async Task<AgencyReviewDto> AddReviewAsync(int agencyId, CreateAgencyReviewDto dto)
    {
        var review = new AgencyReview { AgencyId = agencyId, AuthorName = dto.AuthorName, Rating = dto.Rating, Comment = dto.Comment, CreatedAt = DateTime.UtcNow };
        _context.AgencyReviews.Add(review);
        await _context.SaveChangesAsync();
        return new AgencyReviewDto { Id = review.Id, AuthorName = review.AuthorName, Rating = review.Rating, Comment = review.Comment, CreatedAt = review.CreatedAt };
    }

    public async Task<bool> UpdatePlanAsync(int agencyId, string plan)
    {
        var agency = await _context.Agencies.FindAsync(agencyId);
        if (agency == null) return false;
        agency.Plan = plan;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateResponseTimeAsync(int agencyId, int hours)
    {
        var agency = await _context.Agencies.FindAsync(agencyId);
        if (agency == null) return false;
        agency.AverageResponseHours = hours;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<object>> GetAgentsAsync(int agencyId)
    {
        return await _context.Users
            .Where(u => u.AgencyId == agencyId)
            .Select(u => (object)new { u.Id, u.FullName, u.Email })
            .ToListAsync();
    }
}
