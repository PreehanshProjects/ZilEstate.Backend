using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZilEstate.Application.DTOs;
using ZilEstate.Application.Services;

namespace ZilEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ReportService _reportService;

    public ReportsController(ReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReport([FromBody] CreatePropertyReportDto dto, CancellationToken cancellationToken)
    {
        int? userId = null;
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out var uid))
            userId = uid;

        var report = await _reportService.CreateAsync(dto, userId, cancellationToken);
        return Ok(report);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var reports = await _reportService.GetAllAsync(cancellationToken);
        return Ok(reports);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateReportStatusDto dto, CancellationToken cancellationToken)
    {
        var success = await _reportService.UpdateStatusAsync(id, dto.Status, cancellationToken);
        if (!success) return NotFound();
        return Ok();
    }
}
