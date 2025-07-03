namespace CatalogingSystem.Services.Interfaces;

using CatalogingSystem.DTOs;
using CatalogingSystem.DTOs.Dtos;
using System.Threading.Tasks;

public interface IAuditLogService
{
    Task<PagedResultDto<AuditLogDto>> GetAuditLogsAsync(
        string? tableName = null,
        string? operation = null,
        string? username = null,
        long? expediente = null,
        int page = 1,
        int size = 10);
}