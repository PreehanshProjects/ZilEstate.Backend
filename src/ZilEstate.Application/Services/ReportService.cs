using Microsoft.EntityFrameworkCore;
using ZilEstate.Application.Common.Interfaces;
using ZilEstate.Application.DTOs;
using ZilEstate.Domain.Entities;

namespace ZilEstate.Application.Services;

public class ReportService
{
    private readonly IApplicationDbContext _context;

    public ReportService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PropertyReportDto> CreateAsync(CreatePropertyReportDto dto, int? userId, CancellationToken cancellationToken = default)
    {
        var report = new PropertyReport
        {
            PropertyId = dto.PropertyId,
            ReporterId = userId,
            ReporterName = dto.ReporterName,
            ReporterEmail = dto.ReporterEmail,
            Reason = dto.Reason,
            Details = dto.Details,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.PropertyReports.Add(report);
        await _context.SaveChangesAsync(cancellationToken);

        var property = await _context.Properties.FindAsync(new object[] { dto.PropertyId }, cancellationToken);

        return new PropertyReportDto
        {
            Id = report.Id,
            PropertyId = report.PropertyId,
            PropertyTitle = property?.Title ?? string.Empty,
            ReporterId = report.ReporterId,
            ReporterName = report.ReporterName,
            ReporterEmail = report.ReporterEmail,
            Reason = report.Reason,
            Details = report.Details,
            Status = report.Status,
            CreatedAt = report.CreatedAt
        };
    }

    public async Task<List<PropertyReportDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PropertyReports
            .Include(r => r.Property)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new PropertyReportDto
            {
                Id = r.Id,
                PropertyId = r.PropertyId,
                PropertyTitle = r.Property.Title,
                ReporterId = r.ReporterId,
                ReporterName = r.ReporterName,
                ReporterEmail = r.ReporterEmail,
                Reason = r.Reason,
                Details = r.Details,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> UpdateStatusAsync(int id, string status, CancellationToken cancellationToken = default)
    {
        var report = await _context.PropertyReports.FindAsync(new object[] { id }, cancellationToken);
        if (report == null) return false;

        report.Status = status;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
