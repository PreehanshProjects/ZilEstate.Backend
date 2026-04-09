using Microsoft.EntityFrameworkCore;
using ZilEstate.Application.Common.Interfaces;
using ZilEstate.Application.DTOs;
using ZilEstate.Domain.Entities;

namespace ZilEstate.Application.Services;

public class SavedSearchService
{
    private readonly IApplicationDbContext _context;
    public SavedSearchService(IApplicationDbContext context) { _context = context; }

    public async Task<List<SavedSearchDto>> GetByUserAsync(int userId, CancellationToken ct = default) =>
        await _context.SavedSearches
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SavedSearchDto
            {
                Id = s.Id, Name = s.Name, PropertyType = s.PropertyType, Status = s.Status,
                MinPrice = s.MinPrice, MaxPrice = s.MaxPrice, Bedrooms = s.Bedrooms,
                District = s.District, Keyword = s.Keyword, CreatedAt = s.CreatedAt
            })
            .ToListAsync(ct);

    public async Task<SavedSearchDto> CreateAsync(int userId, CreateSavedSearchDto dto, CancellationToken ct = default)
    {
        var s = new SavedSearch
        {
            UserId = userId, Name = dto.Name, PropertyType = dto.PropertyType,
            Status = dto.Status, MinPrice = dto.MinPrice, MaxPrice = dto.MaxPrice,
            Bedrooms = dto.Bedrooms, District = dto.District, Keyword = dto.Keyword,
            CreatedAt = DateTime.UtcNow
        };
        _context.SavedSearches.Add(s);
        await _context.SaveChangesAsync(ct);
        return new SavedSearchDto { Id = s.Id, Name = s.Name, PropertyType = s.PropertyType, Status = s.Status, MinPrice = s.MinPrice, MaxPrice = s.MaxPrice, Bedrooms = s.Bedrooms, District = s.District, Keyword = s.Keyword, CreatedAt = s.CreatedAt };
    }

    public async Task<bool> DeleteAsync(int userId, int id, CancellationToken ct = default)
    {
        var s = await _context.SavedSearches.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct);
        if (s == null) return false;
        _context.SavedSearches.Remove(s);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}
