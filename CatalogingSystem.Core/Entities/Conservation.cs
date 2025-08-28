namespace CatalogingSystem.Core.Entities;

public class Conservation
{
    public Guid Id { get; set; }
    public required long Expediente { get; set; }
    public required long Inventory { get; set; }
    public string? AffectedArea { get; set; }
    public string? Length { get; set; }
    public string? Width { get; set; }
    public string? Depth { get; set; }
    public string? Reports { get; set; }
    public string? AnalysisTypes { get; set; }
    public string? Results { get; set; }
    public string? TreatmentType { get; set; }
    public string? Description { get; set; }
    public string? SpecialConditions { get; set; }
    public string? Observations { get; set; }
    public string? Notes { get; set; }
    public ArchivoAdministrativo ArchivoAdministrativo { get; set; }
}
