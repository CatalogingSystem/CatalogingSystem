using System;
using System.Threading.Tasks;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogingSystem.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Director")]
public class TenantCustomizationController : ControllerBase
{
    private readonly ITenantCustomizationService _service;

    public TenantCustomizationController(ITenantCustomizationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<TenantCustomizationDto>> GetTenantCustomization()
    {
        var customization = await _service.GetTenantCustomization();
        return customization == null ? NotFound() : Ok(customization);
    }

    [HttpPost]
    public async Task<ActionResult<TenantCustomization>> PostTenantCustomization(
        TenantCustomizationDto dto
    )
    {
        try
        {
            var customization = await _service.CreateTenantCustomization(dto);
            return CreatedAtAction(nameof(GetTenantCustomization), customization);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> PutTenantCustomization(UpdateTenantCustomizationDto dto)
    {
        var success = await _service.UpdateTenantCustomization(dto);
        return success ? NoContent() : NotFound();
    }
}
