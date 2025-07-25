using System.Text.Json;
using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.Data.DbContext;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatalogingSystem.Services.Implementations;

public class DatingService : IDatingService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;

    public DatingService(ApplicationDbContext context, IMapper mapper, IAuditService auditService)
    {
        _context = context;
        _mapper = mapper;
        _auditService = auditService;
    }

    public async Task<IEnumerable<DatingDto>> GetDatings()
    {
        var datings = await _context.Datings
            .Include(d => d.ArchivoAdministrativo)
            .ToListAsync();
        return _mapper.Map<IEnumerable<DatingDto>>(datings);
    }

    public async Task<DatingDto?> GetDating(long expediente)
    {
        var dating = await _context.Datings
            .Include(d => d.ArchivoAdministrativo)
            .FirstOrDefaultAsync(d => d.Expediente == expediente);
        return dating == null ? null : _mapper.Map<DatingDto>(dating);
    }

    public async Task<Dating> CreateDating(DatingDto dto)
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

        bool exists = await _context.Datings
            .AnyAsync(d => d.Expediente == dto.Expediente);
        if (exists)
        {
            throw new InvalidOperationException($"A dating already exists for file number {dto.Expediente}");
        }

        var dating = _mapper.Map<Dating>(dto);
        dating.Id = Guid.NewGuid();
        dating.Inventory = identification.inventory;

        _context.Datings.Add(dating);
        await _auditService.LogAuditAsync("CREATE", dating.Id, null, dating);
        await _context.SaveChangesAsync();

        return dating;
    }

    public async Task<bool> UpdateDating(long expediente, UpdateDatingDto dto)
    {
        var dating = await _context.Datings
            .FirstOrDefaultAsync(d => d.Expediente == expediente);
        if (dating == null) return false;

        var oldDataJson = JsonSerializer.Serialize(dating);
        var oldData = JsonSerializer.Deserialize<Dating>(oldDataJson);

        _mapper.Map(dto, dating);

        await _auditService.LogAuditAsync("UPDATE", dating.Id, oldData, dating);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDating(long expediente)
    {
        var dating = await _context.Datings
            .FirstOrDefaultAsync(d => d.Expediente == expediente);
        if (dating == null) return false;

        _context.Datings.Remove(dating);
        await _context.SaveChangesAsync();
        return true;
    }
}