using Microsoft.EntityFrameworkCore;
using ZilEstate.Application.Common.Interfaces;
using ZilEstate.Application.DTOs;
using ZilEstate.Domain.Entities;

namespace ZilEstate.Application.Services;

public class CollectionService
{
    private readonly IApplicationDbContext _context;

    public CollectionService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CollectionDto>> GetUserCollectionsAsync(int userId, CancellationToken ct = default)
    {
        var collections = await _context.Collections
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Property)
                    .ThenInclude(p => p.Images)
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Property)
                    .ThenInclude(p => p.Location)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

        return collections.Select(MapToDto).ToList();
    }

    public async Task<CollectionDto?> GetByIdAsync(int id, int userId, CancellationToken ct = default)
    {
        var c = await _context.Collections
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Property)
                    .ThenInclude(p => p.Images)
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Property)
                    .ThenInclude(p => p.Location)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, ct);

        return c == null ? null : MapToDto(c);
    }

    public async Task<CollectionDto> CreateAsync(int userId, CreateCollectionDto dto, CancellationToken ct = default)
    {
        var collection = new Collection
        {
            UserId = userId,
            Name = dto.Name,
            Description = dto.Description,
        };
        _context.Collections.Add(collection);
        await _context.SaveChangesAsync(ct);
        return MapToDto(collection);
    }

    public async Task<bool> DeleteAsync(int id, int userId, CancellationToken ct = default)
    {
        var collection = await _context.Collections.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, ct);
        if (collection == null) return false;
        _context.Collections.Remove(collection);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> AddPropertyAsync(int collectionId, int propertyId, int userId, CancellationToken ct = default)
    {
        var collection = await _context.Collections.FirstOrDefaultAsync(c => c.Id == collectionId && c.UserId == userId, ct);
        if (collection == null) return false;

        var exists = await _context.CollectionItems.AnyAsync(ci => ci.CollectionId == collectionId && ci.PropertyId == propertyId, ct);
        if (exists) return true;

        _context.CollectionItems.Add(new CollectionItem { CollectionId = collectionId, PropertyId = propertyId });
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RemovePropertyAsync(int collectionId, int propertyId, int userId, CancellationToken ct = default)
    {
        var collection = await _context.Collections.FirstOrDefaultAsync(c => c.Id == collectionId && c.UserId == userId, ct);
        if (collection == null) return false;

        var item = await _context.CollectionItems.FirstOrDefaultAsync(ci => ci.CollectionId == collectionId && ci.PropertyId == propertyId, ct);
        if (item == null) return false;

        _context.CollectionItems.Remove(item);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    private static CollectionDto MapToDto(Collection c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Description = c.Description,
        ItemCount = c.Items.Count,
        CreatedAt = c.CreatedAt,
        Items = c.Items.Select(ci => new CollectionItemDto
        {
            Id = ci.Id,
            PropertyId = ci.PropertyId,
            PropertyTitle = ci.Property?.Title ?? string.Empty,
            PrimaryImageUrl = ci.Property?.Images.FirstOrDefault(i => i.IsPrimary)?.Url
                           ?? ci.Property?.Images.FirstOrDefault()?.Url,
            Price = ci.Property?.Price ?? 0,
            Status = ci.Property?.Status.ToString() ?? string.Empty,
            District = ci.Property?.Location?.District ?? string.Empty,
            AddedAt = ci.AddedAt,
        }).ToList()
    };
}
