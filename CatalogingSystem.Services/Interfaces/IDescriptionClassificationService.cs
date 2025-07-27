using CatalogingSystem.Core.Entities.DescriptionClassification;
using CatalogingSystem.DTOs.Dtos;

namespace CatalogingSystem.Services.Interfaces;

public interface IDescriptionClassificationService
{
    Task<IEnumerable<DescriptionClassificationDto>> GetDescriptionClassifications();
    Task<DescriptionClassificationDto?> GetDescriptionClassification(long expediente);
    Task<DescriptionClassification> CreateDescriptionClassification(DescriptionClassificationDto dto);
    Task<bool> UpdateDescriptionClassification(long expediente, UpdateDescriptionClassificationDto dto);
    Task<bool> DeleteDescriptionClassification(long expediente);
}