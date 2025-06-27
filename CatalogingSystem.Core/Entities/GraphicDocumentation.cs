namespace CatalogingSystem.Core.Entities;

public class GraphicDocumentation
{
    public Guid Id { get; set; }
    public required long expediente { get; set; }
    public required long inventory { get; set; }
    public string? genericControlNumber { get; set; }
    public string? specificControlNumber { get; set; }
    public DateTime? date { get; set; }
    public List<string>? supportTypes { get; set; }
    public Dimensions? dimensions { get; set; }
    public ImageAuthor? imageAuthor { get; set; }
    public required string description { get; set; }
    public required string technicalData { get; set; }
    public string? generalObservations { get; set; }
    public List<string>? imageUrls { get; set; }
    public required ArchivoAdministrativo ArchivoAdministrativo { get; set; }
    public bool IsModified { get; set; }
    public bool IsCreated { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public List<HistorialEntry> HistorialEntries { get; set; } = new List<HistorialEntry>();
}