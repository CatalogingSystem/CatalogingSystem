using System.Threading.Tasks;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogingSystem.Api.Controllers;

[ApiController]
[Route("metrics")]
[Authorize(Policy = "ArchivoAdminRead")]
public class MetricsController : ControllerBase
{
    private readonly IMetricsService _metricsService;

    public MetricsController(IMetricsService metricsService)
    {
        _metricsService = metricsService;
    }

    /// <summary>
    /// Gets metrics of objects created by Document Origin Type.
    /// </summary>
    [HttpGet("objects-by-document-origin")]
    public async Task<ActionResult<MetricResult<string>>> GetObjectsByDocumentOrigin()
    {
        var results = await _metricsService.GetObjectsByDocumentOriginAsync();
        return Ok(results);
    }

    /// <summary>
    /// Gets metrics of objects created by Institution Type.
    /// </summary>
    [HttpGet("objects-by-institution-type")]
    public async Task<ActionResult<MetricResult<string>>> GetObjectsByInstitutionType()
    {
        var results = await _metricsService.GetObjectsByInstitutionTypeAsync();
        return Ok(results);
    }

    /// <summary>
    /// Gets top users by activity on pieces.
    /// </summary>
    /// <param name="top">Number of top users (default 10).</param>
    [HttpGet("top-active-users")]
    public async Task<ActionResult<MetricResult<string>>> GetTopActiveUsers(
        [FromQuery] int top = 10
    )
    {
        var results = await _metricsService.GetTopActiveUsersOnPiecesAsync(top);
        return Ok(results);
    }

    /// <summary>
    /// Gets top moved pieces by number of temporal movements.
    /// </summary>
    /// <param name="top">Number of top pieces (default 10).</param>
    [HttpGet("top-moved-pieces")]
    public async Task<ActionResult<MetricResult<long>>> GetTopMovedPieces([FromQuery] int top = 10)
    {
        var results = await _metricsService.GetTopMovedPiecesAsync(top);
        return Ok(results);
    }
}
