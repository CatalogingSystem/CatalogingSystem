namespace CatalogingSystem.DTOs.Dtos;

public class BibliographyDto
{
    public string? Title { get; set; }
    public string? RegistrationNumber { get; set; }
    public DateTime? Date { get; set; }
    public string? Author { get; set; }
    public string? SourceDocument { get; set; }
    public string? Page { get; set; }
    public string? TextualCitation { get; set; }
    public string? Notes { get; set; }
}
