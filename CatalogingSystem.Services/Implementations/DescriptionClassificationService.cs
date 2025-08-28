using System.Text.Json;
using AutoMapper;
using CatalogingSystem.Core.Entities.DescriptionClassification;
using CatalogingSystem.Data.DbContext;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatalogingSystem.Services.Implementations;

public class DescriptionClassificationService : IDescriptionClassificationService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;

    public DescriptionClassificationService(
        ApplicationDbContext context,
        IMapper mapper,
        IAuditService auditService
    )
    {
        _context = context;
        _mapper = mapper;
        _auditService = auditService;
    }

    public async Task<IEnumerable<DescriptionClassificationDto>> GetDescriptionClassifications()
    {
        var descriptions = await _context
            .DescriptionClassifications.Include(d => d.ArchivoAdministrativo)
            .ToListAsync();
        return _mapper.Map<IEnumerable<DescriptionClassificationDto>>(descriptions);
    }

    public async Task<DescriptionClassificationDto?> GetDescriptionClassification(long expediente)
    {
        var description = await _context
            .DescriptionClassifications.Include(d => d.ArchivoAdministrativo)
            .FirstOrDefaultAsync(d => d.Expediente == expediente);
        return description == null ? null : _mapper.Map<DescriptionClassificationDto>(description);
    }

    public async Task<DescriptionClassification> CreateDescriptionClassification(
        DescriptionClassificationDto dto
    )
    {
        var archivo = await _context.ArchivosAdministrativos.FirstOrDefaultAsync(a =>
            a.expediente == dto.Expediente
        );
        if (archivo == null)
        {
            throw new InvalidOperationException(
                $"No administrative file exists with expediente {dto.Expediente}"
            );
        }

        bool exists = await _context.DescriptionClassifications.AnyAsync(d =>
            d.Expediente == dto.Expediente
        );
        if (exists)
        {
            throw new InvalidOperationException(
                $"Description classification already exists for expediente {dto.Expediente}"
            );
        }

        var description = _mapper.Map<DescriptionClassification>(dto);
        description.Id = Guid.NewGuid();

        await _auditService.LogAuditAsync("CREATE", description.Id, null, description);
        _context.DescriptionClassifications.Add(description);
        await _context.SaveChangesAsync();

        return description;
    }

    public async Task<bool> UpdateDescriptionClassification(
        long expediente,
        UpdateDescriptionClassificationDto dto
    )
    {
        var description = await _context.DescriptionClassifications.FirstOrDefaultAsync(d =>
            d.Expediente == expediente
        );
        if (description == null)
            return false;

        var oldDataJson = JsonSerializer.Serialize(description);
        var oldData = JsonSerializer.Deserialize<DescriptionClassification>(oldDataJson);
        _mapper.Map(dto, description);

        await _auditService.LogAuditAsync("UPDATE", description.Id, oldData, description);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDescriptionClassification(long expediente)
    {
        var description = await _context.DescriptionClassifications.FirstOrDefaultAsync(d =>
            d.Expediente == expediente
        );
        if (description == null)
            return false;

        _context.DescriptionClassifications.Remove(description);
        await _context.SaveChangesAsync();
        return true;
    }
}
