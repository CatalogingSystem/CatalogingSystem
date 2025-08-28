using System.Text.Json;
using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.Data.DbContext;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatalogingSystem.Services.Implementations;

public class AdministrativeDataService : IAdministrativeDataService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;

    public AdministrativeDataService(
        ApplicationDbContext context,
        IMapper mapper,
        IAuditService auditService
    )
    {
        _context = context;
        _mapper = mapper;
        _auditService = auditService;
    }

    public async Task<IEnumerable<AdministrativeDataDto>> GetAdministrativeData()
    {
        var adminData = await _context
            .AdministrativeData.Include(ad => ad.ArchivoAdministrativo)
            .ToListAsync();
        return _mapper.Map<IEnumerable<AdministrativeDataDto>>(adminData);
    }

    public async Task<AdministrativeDataDto?> GetAdministrativeData(long fileNumber)
    {
        var adminData = await _context
            .AdministrativeData.Include(ad => ad.ArchivoAdministrativo)
            .FirstOrDefaultAsync(ad => ad.FileNumber == fileNumber);
        return adminData == null ? null : _mapper.Map<AdministrativeDataDto>(adminData);
    }

    public async Task<AdministrativeData> CreateAdministrativeData(AdministrativeDataDto dto)
    {
        var archivo = await _context.ArchivosAdministrativos.FirstOrDefaultAsync(a =>
            a.expediente == dto.FileNumber
        );
        if (archivo == null)
        {
            throw new InvalidOperationException(
                $"No administrative file exists with file number {dto.FileNumber}"
            );
        }

        bool exists = await _context.AdministrativeData.AnyAsync(ad =>
            ad.FileNumber == dto.FileNumber
        );
        if (exists)
        {
            throw new InvalidOperationException(
                $"Administrative data already exists for file number {dto.FileNumber}"
            );
        }

        var adminData = _mapper.Map<AdministrativeData>(dto);
        adminData.Id = Guid.NewGuid();

        await _auditService.LogAuditAsync("CREATE", adminData.Id, null, adminData);
        _context.AdministrativeData.Add(adminData);
        await _context.SaveChangesAsync();

        return adminData;
    }

    public async Task<bool> UpdateAdministrativeData(
        long fileNumber,
        UpdateAdministrativeDataDto dto
    )
    {
        var adminData = await _context.AdministrativeData.FirstOrDefaultAsync(ad =>
            ad.FileNumber == fileNumber
        );
        if (adminData == null)
            return false;

        var oldDataJson = JsonSerializer.Serialize(adminData);
        var oldData = JsonSerializer.Deserialize<AdministrativeData>(oldDataJson);
        _mapper.Map(dto, adminData);

        await _auditService.LogAuditAsync("UPDATE", adminData.Id, oldData, adminData);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAdministrativeData(long fileNumber)
    {
        var adminData = await _context.AdministrativeData.FirstOrDefaultAsync(ad =>
            ad.FileNumber == fileNumber
        );
        if (adminData == null)
            return false;

        _context.AdministrativeData.Remove(adminData);
        await _context.SaveChangesAsync();
        return true;
    }
}
