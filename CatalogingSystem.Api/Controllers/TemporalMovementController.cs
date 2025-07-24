using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogingSystem.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TemporalMovementController : ControllerBase
{
    private readonly ITemporalMovementService _service;

    public TemporalMovementController(ITemporalMovementService service)
    {
        _service = service;
    }

    [HttpGet("expediente/{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<IEnumerable<TemporalMovementDto>>> GetTemporalMovementsByExpediente(long expediente)
    {
        return Ok(await _service.GetTemporalMovementsByExpediente(expediente));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<TemporalMovementDto>> GetTemporalMovement(Guid id)
    {
        var movement = await _service.GetTemporalMovement(id);
        return movement == null ? NotFound() : Ok(movement);
    }

    [HttpPost]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<ActionResult<TemporalMovement>> PostTemporalMovement(TemporalMovementDto dto)
    {
        try
        {
            var movement = await _service.CreateTemporalMovement(dto);
            return CreatedAtAction(nameof(GetTemporalMovement), new { id = movement.Id }, movement);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> PutTemporalMovement(Guid id, UpdateTemporalMovementDto dto)
    {
        var success = await _service.UpdateTemporalMovement(id, dto);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> DeleteTemporalMovement(Guid id)
    {
        var success = await _service.DeleteTemporalMovement(id);
        return success ? NoContent() : NotFound();
    }
}