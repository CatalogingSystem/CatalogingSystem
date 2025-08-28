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
    private const int MaxPageSize = 50;

    public TemporalMovementService(
        ApplicationDbContext context,
        IMapper mapper,
        IAuditService auditService
    )
    {
        _context = context;
        _mapper = mapper;
        _auditService = auditService;
    }

    public async Task<PagedResultDto<TemporalMovementDto>> GetTemporalMovementsByExpediente(
        long expediente,
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
            .TemporalMovements.AsNoTracking()
            .Where(m => m.Expediente == expediente)
            .Include(m => m.ArchivoAdministrativo);

        int totalItems = await query.CountAsync();

        var movements = await query
            .OrderBy(m => m.DepartureDate)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var movementDtos = _mapper.Map<List<TemporalMovementDto>>(movements);

        return new PagedResultDto<TemporalMovementDto>
        {
            Items = movementDtos,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)size),
            CurrentPage = page,
            PageSize = size,
        };
    }

    public async Task<IEnumerable<TemporalMovementDto>> GetTemporalMovementsByExpediente(
        long expediente
    )
    {
        var movements = await _context
            .TemporalMovements.Where(m => m.Expediente == expediente)
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
        var archivo = await _context.ArchivosAdministrativos.FirstOrDefaultAsync(a =>
            a.expediente == dto.Expediente
        );
        if (archivo == null)
        {
            throw new InvalidOperationException(
                $"No existe un archivo administrativo con el número de expediente {dto.Expediente}"
            );
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
        if (movement == null)
            return false;

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
        if (movement == null)
            return false;
        await _auditService.LogAuditAsync("DELETE", movement.Id, movement, null);
        _context.TemporalMovements.Remove(movement);
        await _context.SaveChangesAsync();
        return true;
    }
}
