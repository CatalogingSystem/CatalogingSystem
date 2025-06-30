namespace CatalogingSystem.Api.Controllers;

using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

[ApiController]
[Route("[controller]")]
public class IdentificationController : ControllerBase
{
    private readonly IIdentificationService _service;

    public IdentificationController(IIdentificationService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<IEnumerable<IdentificationDto>>> GetIdentifications()
    {
        return Ok(await _service.GetIdentifications());
    }

    [HttpGet("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<IdentificationDto>> GetIdentification(long expediente)
    {
        var identification = await _service.GetIdentification(expediente);
        return identification == null ? NotFound() : Ok(identification);
    }

    [HttpPost]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<ActionResult<Identification>> PostIdentification(IdentificationDto dto)
    {
        try
        {
            var identification = await _service.CreateIdentification(dto);
            return CreatedAtAction(nameof(GetIdentification), new { expediente = identification.expediente }, identification);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> PutIdentification(long expediente, UpdateIdentificationDto dto)
    {
        var success = await _service.UpdateIdentification(expediente, dto);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> DeleteIdentification(long expediente)
    {
        var success = await _service.DeleteIdentification(expediente);
        return success ? NoContent() : NotFound();
    }
    /// <summary>
    /// Obtiene el historial de auditoría para una identificación específica. [cite: 8, 9]
    /// </summary>
    /// <param name="expediente">El número de expediente de la identificación.</param>
    /// <returns>Una lista de entradas de auditoría para la identificación especificada.</returns>
    [HttpGet("{expediente:long}/history")]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetIdentificationHistory(long expediente)
    {
        if (!Request.Headers.TryGetValue("tenant", out StringValues tenantIdFromHeader))
        {
            return BadRequest(new { message = "Header 'tenant' es requerido." });
        }

        try
        {
            var logs = await _service.GetIdentificationHistory(expediente, tenantIdFromHeader.ToString());
            return Ok(logs);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}