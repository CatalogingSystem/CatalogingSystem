namespace CatalogingSystem.Services.Interfaces;

using System.Threading.Tasks;
using CatalogingSystem.DTOs;
using CatalogingSystem.DTOs.Dtos;

public interface IAuditLogService
{
    Task<PagedResultDto<AuditLogDto>> GetAuditLogsAsync(
        string? tableName = null,
        string? operation = null,
        string? username = null,
        long? expediente = null,
        int page = 1,
        int size = 10
    );
}
