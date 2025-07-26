using System.Text.Json;
using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.Data.DbContext;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatalogingSystem.Services.Implementations;

public class ConservationService : IConservationService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;

    public ConservationService(ApplicationDbContext context, IMapper mapper, IAuditService auditService)
    {
        _context = context;
        _mapper = mapper;
        _auditService = auditService;
    }

    public async Task<IEnumerable<ConservationDto>> GetConservations()
    {
        var conservations = await _context.Conservations
            .Include(c => c.ArchivoAdministrativo)
            .ToListAsync();
        return _mapper.Map<IEnumerable<ConservationDto>>(conservations);
    }

    public async Task<ConservationDto?> GetConservation(long expediente)
    {
        var conservation = await _context.Conservations
            .Include(c => c.ArchivoAdministrativo)
            .FirstOrDefaultAsync(c => c.Expediente == expediente);
        return conservation == null ? null : _mapper.Map<ConservationDto>(conservation);
    }

    public async Task<Conservation> CreateConservation(ConservationDto dto)
    {
        var archivo = await _context.ArchivosAdministrativos
            .FirstOrDefaultAsync(a => a.expediente == dto.Expediente);
        if (archivo == null)
        {
            throw new InvalidOperationException($"No administrative file exists with file number {dto.Expediente}");
        }

        var identification = await _context.Identifications
            .FirstOrDefaultAsync(i => i.expediente == dto.Expediente);
        if (identification == null)
        {
            throw new InvalidOperationException($"No identification exists for file number {dto.Expediente}");
        }

        bool exists = await _context.Conservations
            .AnyAsync(c => c.Expediente == dto.Expediente);
        if (exists)
        {
            throw new InvalidOperationException($"A conservation record already exists for file number {dto.Expediente}");
        }

        var conservation = _mapper.Map<Conservation>(dto);
        conservation.Id = Guid.NewGuid();
        conservation.Inventory = identification.inventory;

        _context.Conservations.Add(conservation);
        await _auditService.LogAuditAsync("CREATE", conservation.Id, null, conservation);
        await _context.SaveChangesAsync();

        return conservation;
    }

    public async Task<bool> UpdateConservation(long expediente, UpdateConservationDto dto)
    {
        var conservation = await _context.Conservations
            .FirstOrDefaultAsync(c => c.Expediente == expediente);
        if (conservation == null) return false;

        var oldDataJson = JsonSerializer.Serialize(conservation);
        var oldData = JsonSerializer.Deserialize<Conservation>(oldDataJson);

        _mapper.Map(dto, conservation);

        await _auditService.LogAuditAsync("UPDATE", conservation.Id, oldData, conservation);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteConservation(long expediente)
    {
        var conservation = await _context.Conservations
            .FirstOrDefaultAsync(c => c.Expediente == expediente);
        if (conservation == null) return false;

        _context.Conservations.Remove(conservation);
        await _context.SaveChangesAsync();
        return true;
    }
}