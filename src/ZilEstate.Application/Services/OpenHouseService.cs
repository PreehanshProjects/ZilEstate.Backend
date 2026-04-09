using Microsoft.EntityFrameworkCore;
using ZilEstate.Application.Common.Interfaces;
using ZilEstate.Application.DTOs;
using ZilEstate.Domain.Entities;

namespace ZilEstate.Application.Services;

public class OpenHouseService
{
    private readonly IApplicationDbContext _context;

    public OpenHouseService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<OpenHouseEventDto>> GetByPropertyAsync(int propertyId, CancellationToken ct = default)
    {
        var events = await _context.OpenHouseEvents
            .Include(e => e.Rsvps)
            .Include(e => e.Property)
            .Where(e => e.PropertyId == propertyId && e.EventDate >= DateTime.UtcNow)
            .OrderBy(e => e.EventDate)
            .ToListAsync(ct);

        return events.Select(MapToDto).ToList();
    }

    public async Task<OpenHouseEventDto> CreateAsync(int propertyId, CreateOpenHouseEventDto dto, CancellationToken ct = default)
    {
        var ev = new OpenHouseEvent
        {
            PropertyId = propertyId,
            EventDate = dto.EventDate,
            Description = dto.Description,
            MaxAttendees = dto.MaxAttendees,
        };
        _context.OpenHouseEvents.Add(ev);
        await _context.SaveChangesAsync(ct);

        ev.Property = (await _context.Properties.FindAsync(new object[] { propertyId }, ct))!;
        return MapToDto(ev);
    }

    public async Task<bool> DeleteAsync(int eventId, CancellationToken ct = default)
    {
        var ev = await _context.OpenHouseEvents.FindAsync(new object[] { eventId }, ct);
        if (ev == null) return false;
        _context.OpenHouseEvents.Remove(ev);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<OpenHouseRsvpDto?> RsvpAsync(int eventId, CreateOpenHouseRsvpDto dto, CancellationToken ct = default)
    {
        var ev = await _context.OpenHouseEvents.Include(e => e.Rsvps).FirstOrDefaultAsync(e => e.Id == eventId, ct);
        if (ev == null) return null;

        if (ev.MaxAttendees.HasValue && ev.Rsvps.Count >= ev.MaxAttendees.Value)
            return null;

        var existing = ev.Rsvps.FirstOrDefault(r => r.Email == dto.Email);
        if (existing != null)
            return MapRsvpToDto(existing);

        var rsvp = new OpenHouseRsvp
        {
            EventId = eventId,
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
        };
        _context.OpenHouseRsvps.Add(rsvp);
        await _context.SaveChangesAsync(ct);
        return MapRsvpToDto(rsvp);
    }

    private static OpenHouseEventDto MapToDto(OpenHouseEvent e) => new()
    {
        Id = e.Id,
        PropertyId = e.PropertyId,
        PropertyTitle = e.Property?.Title ?? string.Empty,
        EventDate = e.EventDate,
        Description = e.Description,
        MaxAttendees = e.MaxAttendees,
        RsvpCount = e.Rsvps.Count,
        CreatedAt = e.CreatedAt,
        Rsvps = e.Rsvps.Select(MapRsvpToDto).ToList(),
    };

    private static OpenHouseRsvpDto MapRsvpToDto(OpenHouseRsvp r) => new()
    {
        Id = r.Id,
        Name = r.Name,
        Email = r.Email,
        Phone = r.Phone,
        CreatedAt = r.CreatedAt,
    };
}
