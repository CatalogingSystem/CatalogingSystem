namespace CatalogingSystem.Services.Implementations;

using System.Text.Json;
using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.Data.DbContext;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class GraphicDocumentationService : IGraphicDocumentationService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;

    public GraphicDocumentationService(
        ApplicationDbContext context,
        IMapper mapper,
        IAuditService auditService
    )
    {
        _context = context;
        _mapper = mapper;
        _auditService = auditService;
    }

    public async Task<IEnumerable<GraphicDocumentationDto>> GetGraphicDocumentations()
    {
        var graphicDocs = await _context
            .GraphicDocumentations.Include(g => g.ArchivoAdministrativo)
            .ToListAsync();
        return _mapper.Map<IEnumerable<GraphicDocumentationDto>>(graphicDocs);
    }

    public async Task<GraphicDocumentationDto?> GetGraphicDocumentation(long expediente)
    {
        var graphicDoc = await _context
            .GraphicDocumentations.Include(g => g.ArchivoAdministrativo)
            .FirstOrDefaultAsync(g => g.expediente == expediente);
        return graphicDoc == null ? null : _mapper.Map<GraphicDocumentationDto>(graphicDoc);
    }

    public async Task<GraphicDocumentation> CreateGraphicDocumentation(GraphicDocumentationDto dto)
    {
        var archivo = await _context.ArchivosAdministrativos.FirstOrDefaultAsync(a =>
            a.expediente == dto.Expediente
        );
        if (archivo == null)
        {
            throw new InvalidOperationException(
                $"No existe un archivo administrativo con el número de expediente {dto.Expediente}"
            );
        }

        var identification = await _context.Identifications.FirstOrDefaultAsync(i =>
            i.expediente == dto.Expediente
        );
        if (identification == null)
        {
            throw new InvalidOperationException(
                $"No existe una identificación asociada al expediente {dto.Expediente}"
            );
        }

        bool exists = await _context.GraphicDocumentations.AnyAsync(g =>
            g.expediente == dto.Expediente
        );
        if (exists)
        {
            throw new InvalidOperationException(
                $"Ya existe una documentación gráfica para el expediente {dto.Expediente}"
            );
        }

        if (dto.ImageUrls != null && !dto.ImageUrls.Any())
        {
            throw new InvalidOperationException(
                "Si se proporcionan imágenes, debe haber al menos una URL válida."
            );
        }

        var graphicDoc = _mapper.Map<GraphicDocumentation>(dto);
        graphicDoc.Id = Guid.NewGuid();
        graphicDoc.inventory = identification.inventory;

        _context.GraphicDocumentations.Add(graphicDoc);
        await _auditService.LogAuditAsync("CREATE", graphicDoc.Id, null, graphicDoc); // Auditoría después de agregar, antes de guardar
        await _context.SaveChangesAsync();

        return graphicDoc;
    }

    public async Task<bool> UpdateGraphicDocumentation(
        long expediente,
        UpdateGraphicDocumentationDto dto
    )
    {
        var graphicDoc = await _context.GraphicDocumentations.FirstOrDefaultAsync(g =>
            g.expediente == expediente
        );
        if (graphicDoc == null)
            return false;
        var identification = await _context.Identifications.FirstOrDefaultAsync(i =>
            i.expediente == expediente
        );
        if (identification == null)
        {
            throw new InvalidOperationException(
                $"No existe una identificación asociada al expediente {expediente}"
            );
        }
        if (dto.ImageUrls != null && !dto.ImageUrls.Any())
        {
            throw new InvalidOperationException(
                "Si se proporcionan imágenes, debe haber al menos una URL válida."
            );
        }
        var oldDataJson = JsonSerializer.Serialize(graphicDoc);
        var oldData = JsonSerializer.Deserialize<GraphicDocumentation>(oldDataJson);

        _mapper.Map(dto, graphicDoc);
        graphicDoc.inventory = identification.inventory;

        await _auditService.LogAuditAsync("UPDATE", graphicDoc.Id, oldData, graphicDoc);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteGraphicDocumentation(long expediente)
    {
        var graphicDoc = await _context.GraphicDocumentations.FirstOrDefaultAsync(g =>
            g.expediente == expediente
        );
        if (graphicDoc == null)
            return false;

        _context.GraphicDocumentations.Remove(graphicDoc);
        await _context.SaveChangesAsync();
        return true;
    }
}
