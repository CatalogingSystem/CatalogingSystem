using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.Core.Interfaces;
using CatalogingSystem.Data.DbContext;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatalogingSystem.Services.Implementations;

public class TenantCustomizationService : ITenantCustomizationService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentTenantService _tenantService;

    public TenantCustomizationService(ApplicationDbContext context, IMapper mapper, ICurrentTenantService tenantService)
    {
        _context = context;
        _mapper = mapper;
        _tenantService = tenantService;
    }

    public async Task<TenantCustomizationDto?> GetTenantCustomization()
    {
        var customization = await _context.TenantCustomizations
            .FirstOrDefaultAsync(c => c.TenantId == _tenantService.TenantId);
        return customization == null ? null : _mapper.Map<TenantCustomizationDto>(customization);
    }

    public async Task<TenantCustomization> CreateTenantCustomization(TenantCustomizationDto dto)
    {
        if (string.IsNullOrEmpty(_tenantService.TenantId))
        {
            throw new InvalidOperationException("No tenant context available.");
        }

        bool exists = await _context.TenantCustomizations
            .AnyAsync(c => c.TenantId == _tenantService.TenantId);
        if (exists)
        {
            throw new InvalidOperationException("Tenant customization already exists.");
        }

        var customization = _mapper.Map<TenantCustomization>(dto);
        customization.Id = Guid.NewGuid();
        customization.TenantId = _tenantService.TenantId;

        _context.TenantCustomizations.Add(customization);
        await _context.SaveChangesAsync();

        return customization;
    }

    public async Task<bool> UpdateTenantCustomization(UpdateTenantCustomizationDto dto)
    {
        if (string.IsNullOrEmpty(_tenantService.TenantId))
        {
            throw new InvalidOperationException("No tenant context available.");
        }

        var customization = await _context.TenantCustomizations
            .FirstOrDefaultAsync(c => c.TenantId == _tenantService.TenantId);
        if (customization == null) return false;

        _mapper.Map(dto, customization);
        await _context.SaveChangesAsync();
        return true;
    }
}