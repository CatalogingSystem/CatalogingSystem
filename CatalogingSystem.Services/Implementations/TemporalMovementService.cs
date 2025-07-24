using System.Text.Json;
using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.Data.DbContext;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatalogingSystem.Services.Implementations;

public class TemporalMovementService : ITemporalMovementService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;

    public TemporalMovementService(ApplicationDbContext context, IMapper mapper, IAuditService auditService)
    {
        _context = context;
        _mapper = mapper;
        _auditService = auditService;
    }

    public async Task<IEnumerable<TemporalMovementDto>> GetTemporalMovementsByExpediente(long expediente)
    {
        var movements = await _context.TemporalMovements
            .Where(m => m.Expediente == expediente)
            .ToListAsync();
        return _mapper.Map<IEnumerable<TemporalMovementDto>>(movements);
    }

    public async Task<TemporalMovementDto?> GetTemporalMovement(Guid id)
    {
        var movement = await _context.TemporalMovements.FindAsync(id);
        return movement == null ? null : _mapper.Map<TemporalMovementDto>(movement);
    }

    public async Task<TemporalMovement> CreateTemporalMovement(TemporalMovementDto dto)
    {
        var archivo = await _context.ArchivosAdministrativos
            .FirstOrDefaultAsync(a => a.expediente == dto.Expediente);
        if (archivo == null)
        {
            throw new InvalidOperationException($"No existe un archivo administrativo con el número de expediente {dto.Expediente}");
        }

        var movement = _mapper.Map<TemporalMovement>(dto);
        movement.Id = Guid.NewGuid();

        await _auditService.LogAuditAsync("CREATE", movement.Id, null, movement);
        _context.TemporalMovements.Add(movement);
        await _context.SaveChangesAsync();

        return movement;
    }

    public async Task<bool> UpdateTemporalMovement(Guid id, UpdateTemporalMovementDto dto)
    {
        var movement = await _context.TemporalMovements.FindAsync(id);
        if (movement == null) return false;

        var oldDataJson = JsonSerializer.Serialize(movement);
        var oldData = JsonSerializer.Deserialize<TemporalMovement>(oldDataJson);
        _mapper.Map(dto, movement);

        await _auditService.LogAuditAsync("UPDATE", movement.Id, oldData, movement);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTemporalMovement(Guid id)
    {
        var movement = await _context.TemporalMovements.FindAsync(id);
        if (movement == null) return false;

        _context.TemporalMovements.Remove(movement);
        await _context.SaveChangesAsync();
        return true;
    }
}