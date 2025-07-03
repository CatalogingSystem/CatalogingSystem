namespace CatalogingSystem.Api.Controllers;

using CatalogingSystem.DTOs;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "ArchivoAdminRead")]
public class AuditController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Retrieves paginated audit logs with optional filters.
    /// </summary>
    /// <param name="tableName">Filter by table name (e.g., ArchivoAdministrativo, Identification).</param>
    /// <param name="operation">Filter by operation (CREATE, UPDATE, DELETE).</param>
    /// <param name="username">Filter by username.</param>
    /// <param name="page">The page number (default is 1).</param>
    /// <param name="size">The number of items per page (default is 10).</param>
    /// <returns>A paginated list of audit logs.</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<AuditLogDto>>> GetAuditLogs(
        [FromQuery] string? tableName = null,
        [FromQuery] string? operation = null,
        [FromQuery] string? username = null,
        [FromQuery] long? expediente = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var result = await _auditLogService.GetAuditLogsAsync(tableName, operation, username, expediente, page, size);
        return Ok(result);
    }
}