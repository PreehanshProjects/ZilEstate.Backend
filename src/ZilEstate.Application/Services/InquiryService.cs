using Microsoft.EntityFrameworkCore;
using ZilEstate.Application.Common.Interfaces;
using ZilEstate.Application.DTOs;
using ZilEstate.Domain.Entities;

namespace ZilEstate.Application.Services;

public class InquiryService
{
    private readonly IApplicationDbContext _context;
    public InquiryService(IApplicationDbContext context) { _context = context; }

    public async Task<InquiryDto> CreateAsync(int propertyId, CreateInquiryDto dto, CancellationToken ct = default)
    {
        var property = await _context.Properties.FindAsync(new object[] { propertyId }, ct);
        if (property == null) throw new Exception("Property not found");
        var inq = new Inquiry { PropertyId = propertyId, ContactName = dto.ContactName, ContactEmail = dto.ContactEmail, ContactPhone = dto.ContactPhone, Message = dto.Message, Budget = dto.Budget, IsRead = false, CreatedAt = DateTime.UtcNow };
        _context.Inquiries.Add(inq);
        await _context.SaveChangesAsync(ct);
        return new InquiryDto { Id = inq.Id, PropertyId = propertyId, PropertyTitle = property.Title, ContactName = inq.ContactName, ContactEmail = inq.ContactEmail, ContactPhone = inq.ContactPhone, Message = inq.Message, Budget = inq.Budget, IsRead = inq.IsRead, CreatedAt = inq.CreatedAt };
    }

    public async Task<List<InquiryDto>> GetByPropertyAsync(int propertyId, int userId, bool isAdmin, CancellationToken ct = default)
    {
        var property = await _context.Properties.FindAsync(new object[] { propertyId }, ct);
        if (property == null || (!isAdmin && property.UserId != userId)) return new();
        return await _context.Inquiries
            .Include(i => i.Property)
            .Where(i => i.PropertyId == propertyId)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new InquiryDto { Id = i.Id, PropertyId = i.PropertyId, PropertyTitle = i.Property.Title, ContactName = i.ContactName, ContactEmail = i.ContactEmail, ContactPhone = i.ContactPhone, Message = i.Message, Budget = i.Budget, IsRead = i.IsRead, CreatedAt = i.CreatedAt })
            .ToListAsync(ct);
    }

    public async Task<List<InquiryDto>> GetByOwnerAsync(int ownerId, CancellationToken ct = default) =>
        await _context.Inquiries
            .Include(i => i.Property)
            .Where(i => i.Property.UserId == ownerId)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new InquiryDto { Id = i.Id, PropertyId = i.PropertyId, PropertyTitle = i.Property.Title, ContactName = i.ContactName, ContactEmail = i.ContactEmail, ContactPhone = i.ContactPhone, Message = i.Message, Budget = i.Budget, IsRead = i.IsRead, CreatedAt = i.CreatedAt })
            .ToListAsync(ct);

    public async Task MarkReadAsync(int id, CancellationToken ct = default)
    {
        var inq = await _context.Inquiries.FindAsync(new object[] { id }, ct);
        if (inq != null) { inq.IsRead = true; await _context.SaveChangesAsync(ct); }
    }
}
