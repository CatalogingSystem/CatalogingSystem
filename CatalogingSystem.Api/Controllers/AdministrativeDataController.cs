using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogingSystem.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AdministrativeDataController : ControllerBase
{
    private readonly IAdministrativeDataService _service;

    public AdministrativeDataController(IAdministrativeDataService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<IEnumerable<AdministrativeDataDto>>> GetAdministrativeData()
    {
        return Ok(await _service.GetAdministrativeData());
    }

    [HttpGet("{fileNumber:long}")]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<AdministrativeDataDto>> GetAdministrativeData(long fileNumber)
    {
        var adminData = await _service.GetAdministrativeData(fileNumber);
        return adminData == null ? NotFound() : Ok(adminData);
    }

    [HttpPost]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<ActionResult<AdministrativeData>> PostAdministrativeData(AdministrativeDataDto dto)
    {
        try
        {
            var adminData = await _service.CreateAdministrativeData(dto);
            return CreatedAtAction(nameof(GetAdministrativeData), new { fileNumber = adminData.FileNumber }, adminData);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{fileNumber:long}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> PutAdministrativeData(long fileNumber, UpdateAdministrativeDataDto dto)
    {
        var success = await _service.UpdateAdministrativeData(fileNumber, dto);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{fileNumber:long}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> DeleteAdministrativeData(long fileNumber)
    {
        var success = await _service.DeleteAdministrativeData(fileNumber);
        return success ? NoContent() : NotFound();
    }
}