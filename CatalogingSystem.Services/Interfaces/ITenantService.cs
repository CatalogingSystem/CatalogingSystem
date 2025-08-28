namespace CatalogingSystem.Services.Interfaces;

using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;

public interface ITenantService
{
    Task<Tenant> CreateTenantAsync(CreateTenantRequest request);
    Task<PagedResultDto<TenantDto>> GetAllTenantsAsync(int page = 1, int size = 10);
}
