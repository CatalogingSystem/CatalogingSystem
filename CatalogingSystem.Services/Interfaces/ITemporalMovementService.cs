using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;

namespace CatalogingSystem.Services.Interfaces;

public interface ITemporalMovementService
{
    Task<PagedResultDto<TemporalMovementDto>> GetTemporalMovementsByExpediente(
        long expediente,
        int page = 1,
        int size = 10
    );
    Task<IEnumerable<TemporalMovementDto>> GetTemporalMovementsByExpediente(long expediente);
    Task<TemporalMovementDto?> GetTemporalMovement(Guid id);
    Task<TemporalMovement> CreateTemporalMovement(TemporalMovementDto dto);
    Task<bool> UpdateTemporalMovement(Guid id, UpdateTemporalMovementDto dto);
    Task<bool> DeleteTemporalMovement(Guid id);
}
