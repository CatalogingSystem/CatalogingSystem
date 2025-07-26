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

    /// <summary>
    /// Retrieves paginated temporal movements by expediente.
    /// </summary>
    /// <param name="expediente">The expediente number.</param>
    /// <param name="page">The page number (default is 1).</param>
    /// <param name="size">The number of items per page (default is 10).</param>
    /// <returns>A paginated list of temporal movements with metadata.</returns>
    [HttpGet("expediente/{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<PagedResultDto<TemporalMovementDto>>> GetTemporalMovementsByExpediente(
        long expediente,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var result = await _service.GetTemporalMovementsByExpediente(expediente, page, size);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific temporal movement by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the temporal movement.</param>
    /// <returns>The temporal movement if found; otherwise, NotFound.</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<TemporalMovementDto>> GetTemporalMovement(Guid id)
    {
        var movement = await _service.GetTemporalMovement(id);
        return movement == null ? NotFound() : Ok(movement);
    }

    /// <summary>
    /// Creates a new temporal movement.
    /// </summary>
    /// <param name="dto">The temporal movement data.</param>
    /// <returns>The created temporal movement with its location.</returns>
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

    /// <summary>
    /// Updates an existing temporal movement.
    /// </summary>
    /// <param name="id">The unique identifier of the temporal movement.</param>
    /// <param name="dto">The updated temporal movement data.</param>
    /// <returns>NoContent if successful; otherwise, NotFound.</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> PutTemporalMovement(Guid id, UpdateTemporalMovementDto dto)
    {
        var success = await _service.UpdateTemporalMovement(id, dto);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deletes a temporal movement by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the temporal movement.</param>
    /// <returns>NoContent if successful; otherwise, NotFound.</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> DeleteTemporalMovement(Guid id)
    {
        var success = await _service.DeleteTemporalMovement(id);
        return success ? NoContent() : NotFound();
    }
}