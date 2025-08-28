using CatalogingSystem.Core.Entities.DescriptionClassification;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogingSystem.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DescriptionClassificationController : ControllerBase
{
    private readonly IDescriptionClassificationService _service;

    public DescriptionClassificationController(IDescriptionClassificationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves all description classifications.
    /// </summary>
    /// <returns>A list of description classifications.</returns>
    [HttpGet]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<
        ActionResult<IEnumerable<DescriptionClassificationDto>>
    > GetDescriptionClassifications()
    {
        return Ok(await _service.GetDescriptionClassifications());
    }

    /// <summary>
    /// Retrieves a specific description classification by expediente.
    /// </summary>
    /// <param name="expediente">The expediente number.</param>
    /// <returns>The description classification if found; otherwise, NotFound.</returns>
    [HttpGet("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<DescriptionClassificationDto>> GetDescriptionClassification(
        long expediente
    )
    {
        var description = await _service.GetDescriptionClassification(expediente);
        return description == null ? NotFound() : Ok(description);
    }

    /// <summary>
    /// Creates a new description classification.
    /// </summary>
    /// <param name="dto">The description classification data.</param>
    /// <returns>The created description classification with its location.</returns>
    [HttpPost]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<ActionResult<DescriptionClassification>> PostDescriptionClassification(
        DescriptionClassificationDto dto
    )
    {
        try
        {
            var description = await _service.CreateDescriptionClassification(dto);
            return CreatedAtAction(
                nameof(GetDescriptionClassification),
                new { expediente = description.Expediente },
                description
            );
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing description classification.
    /// </summary>
    /// <param name="expediente">The expediente number.</param>
    /// <param name="dto">The updated description classification data.</param>
    /// <returns>NoContent if successful; otherwise, NotFound.</returns>
    [HttpPut("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> PutDescriptionClassification(
        long expediente,
        UpdateDescriptionClassificationDto dto
    )
    {
        var success = await _service.UpdateDescriptionClassification(expediente, dto);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deletes a description classification by expediente.
    /// </summary>
    /// <param name="expediente">The expediente number.</param>
    /// <returns>NoContent if successful; otherwise, NotFound.</returns>
    [HttpDelete("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> DeleteDescriptionClassification(long expediente)
    {
        var success = await _service.DeleteDescriptionClassification(expediente);
        return success ? NoContent() : NotFound();
    }
}
