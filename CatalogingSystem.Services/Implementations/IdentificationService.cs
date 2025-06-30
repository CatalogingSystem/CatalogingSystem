namespace CatalogingSystem.Services.Implementations;

using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using CatalogingSystem.Data.DbContext;
using CatalogingSystem.DTOs.Mapping;

public class IdentificationService : IIdentificationService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public IdentificationService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<IdentificationDto>> GetIdentifications()
    {
        var identifications = await _context.Identifications
            .Include(i => i.ArchivoAdministrativo)
            .ToListAsync();

        return _mapper.Map<IEnumerable<IdentificationDto>>(identifications);
    }

    public async Task<IdentificationDto?> GetIdentification(long expediente)
    {
        var identification = await _context.Identifications
            .Include(i => i.ArchivoAdministrativo)
            .FirstOrDefaultAsync(i => i.expediente == expediente);

        return identification == null ? null : _mapper.Map<IdentificationDto>(identification);
    }

    public async Task<Identification> CreateIdentification(IdentificationDto dto)
    {
        // Validar que el expediente exista en ArchivosAdministrativos
        var archivo = await _context.ArchivosAdministrativos
            .FirstOrDefaultAsync(a => a.expediente == dto.Expediente);
        if (archivo == null)
        {
            throw new InvalidOperationException($"No existe un archivo administrativo con el número de expediente {dto.Expediente}");
        }

        // Validar que el expediente no exista ya en Identifications
        bool existsExpediente = await _context.Identifications.AnyAsync(i => i.expediente == dto.Expediente);
        if (existsExpediente)
        {
            throw new InvalidOperationException($"Ya existe una identificación con el número de expediente {dto.Expediente}");
        }

        var identification = _mapper.Map<Identification>(dto);
        identification.Id = Guid.NewGuid();

        _context.Identifications.Add(identification);
        await _context.SaveChangesAsync();

        return identification;
    }

    public async Task<bool> UpdateIdentification(long expediente, UpdateIdentificationDto dto)
    {
        var identification = await _context.Identifications
            .Include(i => i.section)
            .Include(i => i.typology)
            .Include(i => i.specificName)
            .Include(i => i.author)
            .Include(i => i.title)
            .Include(i => i.material)
            .Include(i => i.techniques)
            .FirstOrDefaultAsync(i => i.expediente == expediente);

        if (identification == null)
        {
            return false; 
        }
        MappingUtilityIdentification.MapUpdateIdentificationDtoToEntity(dto, identification);

        _context.ChangeTracker.DetectChanges();
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteIdentification(long expediente)
    {
        var identification = await _context.Identifications.FirstOrDefaultAsync(i => i.expediente == expediente);
        if (identification == null) return false;

        _context.Identifications.Remove(identification);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<IEnumerable<AuditLogDto>> GetIdentificationHistory(long expediente, string tenantId)
        {
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new ArgumentException("El TenantId es requerido.");
            }

            var logs = await _context.AuditLogs
                .Where(l => l.TenantId == tenantId &&
                       l.EntityExpediente == expediente &&
                       l.EntityName == "Identification")
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
            return _mapper.Map<IEnumerable<AuditLogDto>>(logs);
        }
    
}