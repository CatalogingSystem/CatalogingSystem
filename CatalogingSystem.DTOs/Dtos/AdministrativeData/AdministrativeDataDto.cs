using CatalogingSystem.Core.Enums;

namespace CatalogingSystem.DTOs.Dtos;

public class AdministrativeDataDto
{
    public required long FileNumber { get; set; }
    public DateTime? EntryDate { get; set; }
    public EntryForm? EntryForm { get; set; }
    public string? EntrySource { get; set; }
    public string? CollectionType { get; set; }
    public CopiesReproductionsDto? CopiesReproductions { get; set; }
    public ValuationDto? Valuation { get; set; }
    public CatalogerDto? Cataloger { get; set; }
    public DateTime? CatalogingDate { get; set; }
    public string? Observations { get; set; }
}
