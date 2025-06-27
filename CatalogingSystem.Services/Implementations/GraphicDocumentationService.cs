namespace CatalogingSystem.Services.Implementations;

using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;
using Microsoft.EntityFrameworkCore;
using CatalogingSystem.Data.DbContext;
using CatalogingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;

public class GraphicDocumentationService : IGraphicDocumentationService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GraphicDocumentationService(ApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<GraphicDocumentationDto>> GetGraphicDocumentations()
    {
        var graphicDocs = await _context.GraphicDocumentations
            .Include(g => g.ArchivoAdministrativo)
            .ToListAsync();
        return _mapper.Map<IEnumerable<GraphicDocumentationDto>>(graphicDocs);
    }

    public async Task<GraphicDocumentationDto?> GetGraphicDocumentation(long expediente)
    {
        var graphicDoc = await _context.GraphicDocumentations
            .Include(g => g.ArchivoAdministrativo)
            .FirstOrDefaultAsync(g => g.expediente == expediente);
        return graphicDoc == null ? null : _mapper.Map<GraphicDocumentationDto>(graphicDoc);
    }

    public async Task<GraphicDocumentation> CreateGraphicDocumentation(GraphicDocumentationDto dto)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = _httpContextAccessor.HttpContext.User.Identity.Name;
        var tenantId = _httpContextAccessor.HttpContext.User.FindFirst("TenantId")?.Value;

        var graphicDoc = _mapper.Map<GraphicDocumentation>(dto);
        graphicDoc.Id = Guid.NewGuid();
        graphicDoc.IsCreated = true;
        graphicDoc.IsModified = false;
        graphicDoc.LastModifiedBy = userId;
        graphicDoc.LastModifiedAt = DateTime.UtcNow;

        _context.GraphicDocumentations.Add(graphicDoc);

        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "GraphicDocumentation",
            EntityId = graphicDoc.expediente.ToString(),
            Action = "Create",
            UserId = userId,
            Username = username,
            TenantId = tenantId,
            Timestamp = DateTime.UtcNow,
            Details = JsonConvert.SerializeObject(dto)
        };

        _context.AuditLogs.Add(auditLog);

        var historialEntry = new HistorialEntry
        {
            Id = Guid.NewGuid(),
            AuditLogId = auditLog.Id,
            AuditLog = auditLog
        };

        graphicDoc.HistorialEntries.Add(historialEntry);

        await _context.SaveChangesAsync();
        return graphicDoc;
    }

    public async Task<bool> UpdateGraphicDocumentation(long expediente, UpdateGraphicDocumentationDto dto)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = _httpContextAccessor.HttpContext.User.Identity.Name;
        var tenantId = _httpContextAccessor.HttpContext.User.FindFirst("TenantId")?.Value;

        var graphicDoc = await _context.GraphicDocumentations.FirstOrDefaultAsync(g => g.expediente == expediente);
        if (graphicDoc == null) return false;

        var oldDto = _mapper.Map<GraphicDocumentationDto>(graphicDoc);
        _mapper.Map(dto, graphicDoc);

        graphicDoc.IsModified = true;
        graphicDoc.LastModifiedBy = userId;
        graphicDoc.LastModifiedAt = DateTime.UtcNow;

        var newDto = _mapper.Map<GraphicDocumentationDto>(dto);
        var changes = GetChanges(oldDto, newDto);

        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "GraphicDocumentation",
            EntityId = graphicDoc.expediente.ToString(),
            Action = "Update",
            UserId = userId,
            Username = username,
            TenantId = tenantId,
            Timestamp = DateTime.UtcNow,
            Details = JsonConvert.SerializeObject(changes)
        };

        _context.AuditLogs.Add(auditLog);

        var historialEntry = new HistorialEntry
        {
            Id = Guid.NewGuid(),
            AuditLogId = auditLog.Id
        };

        graphicDoc.HistorialEntries.Add(historialEntry);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteGraphicDocumentation(long expediente)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = _httpContextAccessor.HttpContext.User.Identity.Name;
        var tenantId = _httpContextAccessor.HttpContext.User.FindFirst("TenantId")?.Value;

        var graphicDoc = await _context.GraphicDocumentations.FirstOrDefaultAsync(g => g.expediente == expediente);
        if (graphicDoc == null) return false;

        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "GraphicDocumentation",
            EntityId = graphicDoc.expediente.ToString(),
            Action = "Delete",
            UserId = userId,
            Username = username,
            TenantId = tenantId,
            Timestamp = DateTime.UtcNow,
            Details = null
        };

        _context.AuditLogs.Add(auditLog);
        _context.GraphicDocumentations.Remove(graphicDoc);

        await _context.SaveChangesAsync();
        return true;
    }

    private Dictionary<string, (string OldValue, string NewValue)> GetChanges(GraphicDocumentationDto oldDto, GraphicDocumentationDto newDto)
    {
        var changes = new Dictionary<string, (string, string)>();
        if (oldDto.GenericControlNumber != newDto.GenericControlNumber)
            changes.Add("GenericControlNumber", (oldDto.GenericControlNumber, newDto.GenericControlNumber));
        if (oldDto.SpecificControlNumber != newDto.SpecificControlNumber)
            changes.Add("SpecificControlNumber", (oldDto.SpecificControlNumber, newDto.SpecificControlNumber));
        if (oldDto.Date != newDto.Date)
            changes.Add("Date", (oldDto.Date?.ToString(), newDto.Date?.ToString()));
        if (!oldDto.SupportTypes.SequenceEqual(newDto.SupportTypes))
            changes.Add("SupportTypes", (JsonConvert.SerializeObject(oldDto.SupportTypes), JsonConvert.SerializeObject(newDto.SupportTypes)));
        if (oldDto.Description != newDto.Description)
            changes.Add("Description", (oldDto.Description, newDto.Description));
        if (oldDto.TechnicalData != newDto.TechnicalData)
            changes.Add("TechnicalData", (oldDto.TechnicalData, newDto.TechnicalData));
        if (oldDto.GeneralObservations != newDto.GeneralObservations)
            changes.Add("GeneralObservations", (oldDto.GeneralObservations, newDto.GeneralObservations));
        if (!oldDto.ImageUrls.SequenceEqual(newDto.ImageUrls))
            changes.Add("ImageUris", (JsonConvert.SerializeObject(oldDto.ImageUrls), JsonConvert.SerializeObject(newDto.ImageUrls)));

        return changes;
    }
}