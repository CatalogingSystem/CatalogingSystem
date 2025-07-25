using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogingSystem.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DatingController : ControllerBase
{
    private readonly IDatingService _service;

    public DatingController(IDatingService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<IEnumerable<DatingDto>>> GetDatings()
    {
        return Ok(await _service.GetDatings());
    }

    [HttpGet("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<DatingDto>> GetDating(long expediente)
    {
        var dating = await _service.GetDating(expediente);
        return dating == null ? NotFound() : Ok(dating);
    }

    [HttpPost]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<ActionResult<Dating>> PostDating(DatingDto dto)
    {
        try
        {
            var dating = await _service.CreateDating(dto);
            return CreatedAtAction(nameof(GetDating), new { expediente = dating.Expediente }, dating);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> PutDating(long expediente, UpdateDatingDto dto)
    {
        var success = await _service.UpdateDating(expediente, dto);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> DeleteDating(long expediente)
    {
        var success = await _service.DeleteDating(expediente);
        return success ? NoContent() : NotFound();
    }
}