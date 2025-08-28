using CatalogingSystem.Core.Enums;

namespace CatalogingSystem.Core.Entities;

public class AdministrativeData
{
    public Guid Id { get; set; }
    public required long FileNumber { get; set; }
    public DateTime? EntryDate { get; set; }
    public EntryForm? EntryForm { get; set; }
    public string? EntrySource { get; set; }
    public string? CollectionType { get; set; }
    public CopiesReproductions? CopiesReproductions { get; set; }
    public Valuation? Valuation { get; set; }
    public Cataloger? Cataloger { get; set; }
    public DateTime? CatalogingDate { get; set; }
    public string? Observations { get; set; }
    public ArchivoAdministrativo ArchivoAdministrativo { get; set; }
}
