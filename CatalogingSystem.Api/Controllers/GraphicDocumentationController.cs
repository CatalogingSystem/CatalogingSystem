namespace CatalogingSystem.Api.Controllers;

using CatalogingSystem.Core.Entities;
using CatalogingSystem.Data.DbContext;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("[controller]")]
public class GraphicDocumentationController : ControllerBase
{
    private readonly IGraphicDocumentationService _service;
    private readonly ApplicationDbContext _context;

    public GraphicDocumentationController(IGraphicDocumentationService service, ApplicationDbContext context)
    {
        _service = service;
        _context = context;
    }

    [HttpGet]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<IEnumerable<GraphicDocumentationDto>>> GetGraphicDocumentations()
    {
        return Ok(await _service.GetGraphicDocumentations());
    }

    [HttpGet("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<GraphicDocumentationDto>> GetGraphicDocumentation(long expediente)
    {
        var graphicDoc = await _service.GetGraphicDocumentation(expediente);
        return graphicDoc == null ? NotFound() : Ok(graphicDoc);
    }

    [HttpPost]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<ActionResult<GraphicDocumentation>> PostGraphicDocumentation([FromBody] GraphicDocumentationDto dto)
    {
        try
        {
            var graphicDoc = await _service.CreateGraphicDocumentation(dto);
            return CreatedAtAction(nameof(GetGraphicDocumentation), new { expediente = graphicDoc.expediente }, graphicDoc);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> PutGraphicDocumentation(long expediente, [FromBody] UpdateGraphicDocumentationDto dto)
    {
        try
        {
            var success = await _service.UpdateGraphicDocumentation(expediente, dto);
            return success ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> DeleteGraphicDocumentation(long expediente)
    {
        var success = await _service.DeleteGraphicDocumentation(expediente);
        return success ? NoContent() : NotFound();
    }

    [HttpGet("{expediente:long}/history")]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult<PagedResultDto<HistorialEntryDto>>> GetHistorial(
        long expediente,
        [FromQuery] string? username = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var graphicDoc = await _context.GraphicDocumentations
            .Include(g => g.HistorialEntries)
            .ThenInclude(h => h.AuditLog)
            .FirstOrDefaultAsync(g => g.expediente == expediente);

        if (graphicDoc == null)
        {
            return NotFound();
        }

        var query = graphicDoc.HistorialEntries.AsQueryable();

        if (!string.IsNullOrEmpty(username))
            query = query.Where(h => h.AuditLog.Username == username);
        if (startDate.HasValue)
            query = query.Where(h => h.AuditLog.Timestamp >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(h => h.AuditLog.Timestamp <= endDate.Value);

        var totalItems = query.Count();

        var historialEntries = query
            .OrderByDescending(h => h.AuditLog.Timestamp)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(h => new HistorialEntryDto
            {
                Timestamp = h.AuditLog.Timestamp,
                Username = h.AuditLog.Username,
                Action = h.AuditLog.Action,
                Details = h.AuditLog.Details
            })
            .ToList();

        return Ok(new PagedResultDto<HistorialEntryDto>
        {
            Items = historialEntries,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)size),
            CurrentPage = page,
            PageSize = size
        });
    }
}