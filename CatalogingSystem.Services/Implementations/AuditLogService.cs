namespace CatalogingSystem.Services.Implementations;

using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.Core.Interfaces;
using CatalogingSystem.Data.DbContext;
using CatalogingSystem.DTOs;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class AuditLogService : IAuditLogService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentTenantService _tenantService;
    private const int MaxPageSize = 100;

    public AuditLogService(
        ApplicationDbContext context,
        IMapper mapper,
        ICurrentTenantService tenantService
    )
    {
        _context = context;
        _mapper = mapper;
        _tenantService = tenantService;
    }

    public async Task<PagedResultDto<AuditLogDto>> GetAuditLogsAsync(
        string? tableName = null,
        string? operation = null,
        string? username = null,
        long? expediente = null,
        int page = 1,
        int size = 10
    )
    {
        if (page < 1)
            page = 1;
        if (size < 1)
            size = 10;
        if (size > MaxPageSize)
            size = MaxPageSize;

        var query = _context
            .AuditLogs.AsNoTracking()
            .Where(a => a.TenantId == _tenantService.TenantId);
        if (!string.IsNullOrEmpty(tableName))
        {
            query = query.Where(a => EF.Functions.ILike(a.TableName, $"%{tableName}%"));
        }
        else
        {
            query = query.Where(a => a.TableName != "TemporalMovement");
        }

        if (!string.IsNullOrEmpty(operation))
        {
            query = query.Where(a => EF.Functions.ILike(a.Operation, $"%{operation}%"));
        }

        if (!string.IsNullOrEmpty(username))
        {
            query = query.Where(a => EF.Functions.ILike(a.Username, $"%{username}%"));
        }

        if (expediente.HasValue)
        {
            query = query.Where(a => a.Expediente == expediente.Value);
        }

        int totalItems = await query.CountAsync();

        var auditLogs = await query
            .OrderByDescending(a => a.ActionTimestamp)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var auditLogDtos = _mapper.Map<IEnumerable<AuditLogDto>>(auditLogs);

        return new PagedResultDto<AuditLogDto>
        {
            Items = auditLogDtos,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)size),
            CurrentPage = page,
            PageSize = size,
        };
    }
}
