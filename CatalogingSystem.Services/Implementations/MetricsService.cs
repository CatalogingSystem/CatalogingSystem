using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.Data.DbContext;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatalogingSystem.Services.Implementations;

public class MetricsService : IMetricsService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditLogService _auditLogService;
    private readonly ITemporalMovementService _temporalMovementService;

    public MetricsService(
        ApplicationDbContext context,
        IAuditLogService auditLogService,
        ITemporalMovementService temporalMovementService
    )
    {
        _context = context;
        _auditLogService = auditLogService;
        _temporalMovementService = temporalMovementService;
    }

    public async Task<MetricResult<string>> GetObjectsByDocumentOriginAsync()
    {
        var total = await _context.ArchivosAdministrativos.CountAsync();

        var queryResults = await _context
            .ArchivosAdministrativos.GroupBy(a => a.documentoOrigen) // Replace with actual field name if different
            .Select(g => new { Key = g.Key.ToString(), Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        var items = queryResults.Select(x => new MetricItem<string>
        {
            Key = x.Key,
            Count = x.Count,
            Percentage = total > 0 ? (double)x.Count / total * 100 : 0,
        });

        return new MetricResult<string> { Total = total, Items = items };
    }

    public async Task<MetricResult<string>> GetObjectsByInstitutionTypeAsync()
    {
        var total = await _context.ArchivosAdministrativos.CountAsync();

        var queryResults = await _context
            .ArchivosAdministrativos.GroupBy(a => a.institucion) // Replace with actual field name if different
            .Select(g => new { Key = g.Key.ToString(), Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        var items = queryResults.Select(x => new MetricItem<string>
        {
            Key = x.Key,
            Count = x.Count,
            Percentage = total > 0 ? (double)x.Count / total * 100 : 0,
        });

        return new MetricResult<string> { Total = total, Items = items };
    }

    public async Task<MetricResult<string>> GetTopActiveUsersOnPiecesAsync(int top = 10)
    {
        var pieceTables = new[]
        {
            "ArchivoAdministrativo",
            "Identification",
            "Conservation",
            "Dating",
            "DescriptionClassification",
            "GraphicDocumentation",
        };

        var total = await _context
            .AuditLogs.Where(log => pieceTables.Contains(log.TableName) && log.Operation != null)
            .CountAsync();

        var queryResults = await _context
            .AuditLogs.Where(log => pieceTables.Contains(log.TableName) && log.Operation != null)
            .GroupBy(log => log.Username)
            .Select(g => new { Key = g.Key ?? "Unknown", Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(top)
            .ToListAsync();

        var items = queryResults.Select(x => new MetricItem<string>
        {
            Key = x.Key,
            Count = x.Count,
            Percentage = total > 0 ? (double)x.Count / total * 100 : 0,
        });

        return new MetricResult<string> { Total = total, Items = items };
    }

    public async Task<MetricResult<long>> GetTopMovedPiecesAsync(int top = 10)
    {
        var total = await _context.TemporalMovements.CountAsync();

        var queryResults = await _context
            .TemporalMovements.GroupBy(m => m.Expediente)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(top)
            .ToListAsync();

        var items = queryResults.Select(x => new MetricItem<long>
        {
            Key = x.Key,
            Count = x.Count,
            Percentage = total > 0 ? (double)x.Count / total * 100 : 0,
        });

        return new MetricResult<long> { Total = total, Items = items };
    }
}
