using CatalogingSystem.DTOs.Dtos;

namespace CatalogingSystem.Services.Interfaces;

public interface IMetricsService
{
    Task<MetricResult<string>> GetObjectsByDocumentOriginAsync();
    Task<MetricResult<string>> GetObjectsByInstitutionTypeAsync();
    Task<MetricResult<string>> GetTopActiveUsersOnPiecesAsync(int top = 10);
    Task<MetricResult<long>> GetTopMovedPiecesAsync(int top = 10);
}
