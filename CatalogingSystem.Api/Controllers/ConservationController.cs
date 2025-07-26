using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogingSystem.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ConservationController : ControllerBase
{
    private readonly IConservationService _service;

    public ConservationController(IConservationService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<IEnumerable<ConservationDto>>> GetConservations()
    {
        return Ok(await _service.GetConservations());
    }

    [HttpGet("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<ConservationDto>> GetConservation(long expediente)
    {
        var conservation = await _service.GetConservation(expediente);
        return conservation == null ? NotFound() : Ok(conservation);
    }

    [HttpPost]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<ActionResult<Conservation>> PostConservation(ConservationDto dto)
    {
        try
        {
            var conservation = await _service.CreateConservation(dto);
            return CreatedAtAction(nameof(GetConservation), new { expediente = conservation.Expediente }, conservation);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> PutConservation(long expediente, UpdateConservationDto dto)
    {
        var success = await _service.UpdateConservation(expediente, dto);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> DeleteConservation(long expediente)
    {
        var success = await _service.DeleteConservation(expediente);
        return success ? NoContent() : NotFound();
    }
}