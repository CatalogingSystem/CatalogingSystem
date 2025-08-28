using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;

namespace CatalogingSystem.Services.Interfaces;

public interface IAdministrativeDataService
{
    Task<IEnumerable<AdministrativeDataDto>> GetAdministrativeData();
    Task<AdministrativeDataDto?> GetAdministrativeData(long fileNumber);
    Task<AdministrativeData> CreateAdministrativeData(AdministrativeDataDto dto);
    Task<bool> UpdateAdministrativeData(long fileNumber, UpdateAdministrativeDataDto dto);
    Task<bool> DeleteAdministrativeData(long fileNumber);
}
