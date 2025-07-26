using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;

namespace CatalogingSystem.Services.Interfaces;

public interface IConservationService
{
    Task<IEnumerable<ConservationDto>> GetConservations();
    Task<ConservationDto?> GetConservation(long expediente);
    Task<Conservation> CreateConservation(ConservationDto dto);
    Task<bool> UpdateConservation(long expediente, UpdateConservationDto dto);
    Task<bool> DeleteConservation(long expediente);
}