using Microsoft.EntityFrameworkCore;
using ZilEstate.Application.Common.Interfaces;
using ZilEstate.Application.DTOs;
using ZilEstate.Domain.Entities;

namespace ZilEstate.Application.Services;

public class ViewingRequestService
{
    private readonly IApplicationDbContext _context;
    public ViewingRequestService(IApplicationDbContext context) { _context = context; }

    public async Task<ViewingRequestDto> CreateAsync(int propertyId, int? userId, CreateViewingRequestDto dto, CancellationToken ct = default)
    {
        var property = await _context.Properties.FindAsync(new object[] { propertyId }, ct);
        if (property == null) throw new Exception("Property not found");
        var vr = new ViewingRequest
        {
            PropertyId = propertyId, UserId = userId, ContactName = dto.ContactName,
            ContactEmail = dto.ContactEmail, ContactPhone = dto.ContactPhone,
            PreferredDate = dto.PreferredDate, Message = dto.Message,
            Status = "Pending", CreatedAt = DateTime.UtcNow
        };
        _context.ViewingRequests.Add(vr);
        await _context.SaveChangesAsync(ct);
        return new ViewingRequestDto { Id = vr.Id, PropertyId = propertyId, PropertyTitle = property.Title, ContactName = vr.ContactName, ContactEmail = vr.ContactEmail, ContactPhone = vr.ContactPhone, PreferredDate = vr.PreferredDate, Message = vr.Message, Status = vr.Status, CreatedAt = vr.CreatedAt };
    }

    public async Task<List<ViewingRequestDto>> GetByPropertyAsync(int propertyId, int userId, bool isAdmin, CancellationToken ct = default)
    {
        var property = await _context.Properties.FindAsync(new object[] { propertyId }, ct);
        if (property == null || (!isAdmin && property.UserId != userId)) return new();
        return await _context.ViewingRequests
            .Include(vr => vr.Property)
            .Where(vr => vr.PropertyId == propertyId)
            .OrderByDescending(vr => vr.CreatedAt)
            .Select(vr => new ViewingRequestDto { Id = vr.Id, PropertyId = vr.PropertyId, PropertyTitle = vr.Property.Title, ContactName = vr.ContactName, ContactEmail = vr.ContactEmail, ContactPhone = vr.ContactPhone, PreferredDate = vr.PreferredDate, Message = vr.Message, Status = vr.Status, CreatedAt = vr.CreatedAt })
            .ToListAsync(ct);
    }

    public async Task<List<ViewingRequestDto>> GetByOwnerAsync(int ownerId, CancellationToken ct = default) =>
        await _context.ViewingRequests
            .Include(vr => vr.Property)
            .Where(vr => vr.Property.UserId == ownerId)
            .OrderByDescending(vr => vr.CreatedAt)
            .Select(vr => new ViewingRequestDto { Id = vr.Id, PropertyId = vr.PropertyId, PropertyTitle = vr.Property.Title, ContactName = vr.ContactName, ContactEmail = vr.ContactEmail, ContactPhone = vr.ContactPhone, PreferredDate = vr.PreferredDate, Message = vr.Message, Status = vr.Status, CreatedAt = vr.CreatedAt })
            .ToListAsync(ct);

    public async Task<bool> UpdateStatusAsync(int id, int userId, bool isAdmin, string status, CancellationToken ct = default)
    {
        var vr = await _context.ViewingRequests.Include(v => v.Property).FirstOrDefaultAsync(v => v.Id == id, ct);
        if (vr == null || (!isAdmin && vr.Property.UserId != userId)) return false;
        vr.Status = status;
        await _context.SaveChangesAsync(ct);
        return true;
    }
}
