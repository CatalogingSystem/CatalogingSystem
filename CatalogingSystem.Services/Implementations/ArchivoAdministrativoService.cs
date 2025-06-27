namespace CatalogingSystem.Services.Implementations;

using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using CatalogingSystem.Data.DbContext;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;
using CatalogingSystem.Core.Interfaces;

public class ArchivoAdministrativoService : IArchivoAdministrativoService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentTenantService _tenantService;

    public ArchivoAdministrativoService(
        ApplicationDbContext context, 
        IMapper mapper, 
        IHttpContextAccessor httpContextAccessor, 
        ICurrentTenantService tenantService)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _tenantService = tenantService;
    }

    public async Task<IEnumerable<ArchivoAdministrativoDto>> GetArchivosAdministrativos()
    {
        var archivos = await _context.ArchivosAdministrativos.ToListAsync();
        return _mapper.Map<IEnumerable<ArchivoAdministrativoDto>>(archivos);
    }

    public async Task<ArchivoAdministrativoDto?> GetArchivoAdministrativo(long expediente)
    {
        var archivo = await _context.ArchivosAdministrativos.FirstOrDefaultAsync(a => a.expediente == expediente);
        return archivo == null ? null : _mapper.Map<ArchivoAdministrativoDto>(archivo);
    }

    public async Task<ArchivoAdministrativo> CreateArchivoAdministrativo(ArchivoAdministrativoDto dto)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = _httpContextAccessor.HttpContext.User.Identity.Name;
        var tenantId = _httpContextAccessor.HttpContext.User.FindFirst("tenantId")?.Value;

        if (string.IsNullOrEmpty(tenantId))
        {
            throw new InvalidOperationException("No se pudo obtener el TenantId del usuario autenticado.");
        }

        bool existeExpediente = await _context.ArchivosAdministrativos.AnyAsync(a => a.expediente == dto.Expediente);
        if (existeExpediente)
        {
            throw new InvalidOperationException($"Ya existe un archivo con el número de expediente {dto.Expediente}");
        }

        var archivo = _mapper.Map<ArchivoAdministrativo>(dto);
        archivo.Id = Guid.NewGuid();
        archivo.IsCreated = true;
        archivo.IsModified = false;
        archivo.LastModifiedBy = userId;
        archivo.LastModifiedAt = DateTime.UtcNow; // Asegurar valor inicial

        _context.ArchivosAdministrativos.Add(archivo);

        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "ArchivoAdministrativo",
            EntityId = archivo.expediente.ToString(),
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

        archivo.HistorialEntries.Add(historialEntry);

        await _context.SaveChangesAsync();

        return archivo;
    }

    public async Task<bool> UpdateArchivoAdministrativo(long expediente, ArchivoAdministrativoUpdateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Buscar el archivo administrativo
            var archivo = await _context.ArchivosAdministrativos
                .Include(a => a.HistorialEntries)
                .ThenInclude(h => h.AuditLog)
                .FirstOrDefaultAsync(a => a.expediente == expediente);

            if (archivo == null) return false;

            // Guardar el estado anterior para el historial
            var oldDto = _mapper.Map<ArchivoAdministrativoDto>(archivo);

            // Actualizar el registro con los nuevos datos
            _mapper.Map(dto, archivo);
            archivo.IsModified = true;
            archivo.LastModifiedAt = DateTime.UtcNow;

            // Calcular los cambios para el historial
            var changes = GetChanges(oldDto, _mapper.Map<ArchivoAdministrativoDto>(archivo));

            // Crear el registro de auditoría
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityType = "ArchivoAdministrativo",
                EntityId = archivo.Id.ToString(),
                Action = "Update",
                Timestamp = DateTime.UtcNow,
                UserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Username = _httpContextAccessor.HttpContext?.User.Identity?.Name,
                Details = JsonConvert.SerializeObject(changes),
                TenantId = _tenantService.TenantId
            };
            _context.AuditLogs.Add(auditLog);

            // Agregar entrada al historial
            var historialEntry = new HistorialEntry
            {
                Id = Guid.NewGuid(),
                AuditLogId = auditLog.Id
            };
            archivo.HistorialEntries.Add(historialEntry);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("Error al actualizar el archivo administrativo", ex);
        }
    }

    public async Task<bool> DeleteArchivoAdministrativo(long expediente)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = _httpContextAccessor.HttpContext.User.Identity.Name;
        var tenantId = _httpContextAccessor.HttpContext.User.FindFirst("tenantId")?.Value;

        if (string.IsNullOrEmpty(tenantId))
        {
            throw new InvalidOperationException("No se pudo obtener el TenantId del usuario autenticado.");
        }

        var archivo = await _context.ArchivosAdministrativos.FirstOrDefaultAsync(a => a.expediente == expediente);
        if (archivo == null) return false;

        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "ArchivoAdministrativo",
            EntityId = archivo.expediente.ToString(),
            Action = "Delete",
            UserId = userId,
            Username = username,
            TenantId = tenantId,
            Timestamp = DateTime.UtcNow,
            Details = null
        };

        _context.AuditLogs.Add(auditLog);

        var historialEntry = new HistorialEntry
        {
            Id = Guid.NewGuid(),
            AuditLogId = auditLog.Id,
            AuditLog = auditLog
        };

        archivo.HistorialEntries.Add(historialEntry);

        _context.ArchivosAdministrativos.Remove(archivo);

        await _context.SaveChangesAsync();
        return true;
    }

    private Dictionary<string, (string OldValue, string NewValue)> GetChanges(ArchivoAdministrativoDto oldDto, ArchivoAdministrativoDto newDto)
    {
        var changes = new Dictionary<string, (string, string)>();

        if (oldDto.Institucion != newDto.Institucion)
            changes.Add("Institucion", (oldDto.Institucion.ToString(), newDto.Institucion.ToString()));
        if (oldDto.Unidad != newDto.Unidad)
            changes.Add("Unidad", (oldDto.Unidad, newDto.Unidad));
        if (oldDto.Serie != newDto.Serie)
            changes.Add("Serie", (oldDto.Serie, newDto.Serie));
        if (oldDto.DocumentoOrigen != newDto.DocumentoOrigen)
            changes.Add("DocumentoOrigen", (oldDto.DocumentoOrigen.ToString(), newDto.DocumentoOrigen.ToString()));
        if (oldDto.FechaInicial != newDto.FechaInicial)
            changes.Add("FechaInicial", (oldDto.FechaInicial.ToString(), newDto.FechaInicial.ToString()));
        if (oldDto.FechaFinal != newDto.FechaFinal)
            changes.Add("FechaFinal", (oldDto.FechaFinal?.ToString(), newDto.FechaFinal?.ToString()));
        if (oldDto.ExpedienteAnterior != newDto.ExpedienteAnterior)
            changes.Add("ExpedienteAnterior", (oldDto.ExpedienteAnterior, newDto.ExpedienteAnterior));
        if (oldDto.Asunto != newDto.Asunto)
            changes.Add("Asunto", (oldDto.Asunto, newDto.Asunto));
        if (oldDto.PeticionTransferencia != newDto.PeticionTransferencia)
            changes.Add("PeticionTransferencia", (oldDto.PeticionTransferencia.ToString(), newDto.PeticionTransferencia.ToString()));
        if (oldDto.Historial != newDto.Historial)
            changes.Add("Historial", (oldDto.Historial, newDto.Historial));
        if (oldDto.ArchivoDocumental != newDto.ArchivoDocumental)
            changes.Add("ArchivoDocumental", (oldDto.ArchivoDocumental, newDto.ArchivoDocumental));
        if (oldDto.Observaciones != newDto.Observaciones)
            changes.Add("Observaciones", (oldDto.Observaciones, newDto.Observaciones));

        return changes;
    }
}