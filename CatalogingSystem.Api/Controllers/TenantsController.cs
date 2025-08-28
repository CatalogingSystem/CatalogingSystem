using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogingSystem.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "SuperDirector")]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantsController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    /// <summary>
    /// Gets all tenants with pagination.
    /// </summary>
    /// <param name="page">The page number (default is 1).</param>
    /// <param name="size">The number of items per page (default is 10).</param>
    /// <returns>A paginated list of tenants.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllTenants(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10
    )
    {
        try
        {
            var pagedTenants = await _tenantService.GetAllTenantsAsync(page, size);
            return Ok(pagedTenants);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error retrieving tenants: {ex.Message}" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequest request)
    {
        try
        {
            var tenant = await _tenantService.CreateTenantAsync(request);
            return CreatedAtAction(nameof(GetAllTenants), new { id = tenant.Id }, tenant);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error creating tenant: {ex.Message}" });
        }
    }
}
