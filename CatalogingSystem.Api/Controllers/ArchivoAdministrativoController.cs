using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CatalogingSystem.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ArchivoAdministrativoController : ControllerBase
{
    private readonly IArchivoAdministrativoService _service;

    public ArchivoAdministrativoController(IArchivoAdministrativoService service)
    {
        _service = service;
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
    public async Task<IActionResult> PutArchivoAdministrativo(long expediente, ArchivoAdministrativoDto dto)
    {
        var success = await _service.UpdateArchivoAdministrativo(expediente, dto);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> DeleteArchivoAdministrativo(long expediente)
    {
        var success = await _service.DeleteArchivoAdministrativo(expediente);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Obtiene el historial de auditoría para un archivo administrativo específico.
    /// </summary>
    /// <param name="expediente">El número de expediente del archivo administrativo.</param>
    /// <returns>Una lista de entradas de auditoría para el archivo especificado.</returns>
    [HttpGet("{expediente:long}/history")]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetArchivoAdministrativoHistory(long expediente)
    {
        if (!Request.Headers.TryGetValue("tenant", out StringValues tenantIdFromHeader))
        {
            return BadRequest(new { message = "Header 'tenant' es requerido." });
        }

        try
        {
            var logs = await _service.GetArchivoAdministrativoHistory(expediente, tenantIdFromHeader.ToString());
            return Ok(logs);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}