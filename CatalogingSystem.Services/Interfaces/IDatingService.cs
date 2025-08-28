using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;

namespace CatalogingSystem.Services.Interfaces;

public interface IDatingService
{
    Task<IEnumerable<DatingDto>> GetDatings();
    Task<DatingDto?> GetDating(long expediente);
    Task<Dating> CreateDating(DatingDto dto);
    Task<bool> UpdateDating(long expediente, UpdateDatingDto dto);
    Task<bool> DeleteDating(long expediente);
}
