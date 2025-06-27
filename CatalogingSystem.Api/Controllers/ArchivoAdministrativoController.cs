using CatalogingSystem.Core.Entities;
using CatalogingSystem.Data.DbContext;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogingSystem.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ArchivoAdministrativoController : ControllerBase
{
    private readonly IArchivoAdministrativoService _service;
    private readonly ApplicationDbContext _context;

    public ArchivoAdministrativoController(IArchivoAdministrativoService service, ApplicationDbContext context)
    {
        _service = service;
        _context = context;
    }

    [HttpGet]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<IEnumerable<ArchivoAdministrativoDto>>> GetArchivosAdministrativos()
    {
        return Ok(await _service.GetArchivosAdministrativos());
    }

    [HttpGet("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<ArchivoAdministrativoDto>> GetArchivoAdministrativo(long expediente)
    {
        var archivo = await _service.GetArchivoAdministrativo(expediente);
        return archivo == null ? NotFound() : Ok(archivo);
    }

    [HttpPost]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<ActionResult<ArchivoAdministrativo>> PostArchivoAdministrativo(ArchivoAdministrativoDto dto)
    {
        try
        {
            var archivo = await _service.CreateArchivoAdministrativo(dto);
            return CreatedAtAction(nameof(GetArchivoAdministrativo), new { expediente = archivo.expediente }, archivo);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> PutArchivoAdministrativo(long expediente, ArchivoAdministrativoUpdateDto dto)
    {
        try
        {
            var success = await _service.UpdateArchivoAdministrativo(expediente, dto);
            if (!success)
            {
                return NotFound(new { message = $"No se encontró un archivo administrativo con expediente {expediente}." });
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Ocurrió un error al actualizar el archivo administrativo: {ex.Message}" });
        }
    }

    [HttpDelete("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> DeleteArchivoAdministrativo(long expediente)
    {
        var success = await _service.DeleteArchivoAdministrativo(expediente);
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
        var archivo = await _context.ArchivosAdministrativos
            .Include(a => a.HistorialEntries)
            .ThenInclude(h => h.AuditLog)
            .FirstOrDefaultAsync(a => a.expediente == expediente);

        if (archivo == null)
        {
            return NotFound();
        }

        var query = archivo.HistorialEntries.AsQueryable();

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