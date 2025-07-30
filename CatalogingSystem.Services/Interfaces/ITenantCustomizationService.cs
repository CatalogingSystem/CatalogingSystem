using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;

namespace CatalogingSystem.Services.Interfaces;

public interface ITenantCustomizationService
{
    Task<TenantCustomizationDto?> GetTenantCustomization();
    Task<TenantCustomization> CreateTenantCustomization(TenantCustomizationDto dto);
    Task<bool> UpdateTenantCustomization(UpdateTenantCustomizationDto dto);
}