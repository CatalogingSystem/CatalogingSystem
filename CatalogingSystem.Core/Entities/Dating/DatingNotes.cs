namespace CatalogingSystem.Core.Entities;

public class DatingNotes
{
    public bool IsPresent { get; set; } = true;
    public string? TextualDate { get; set; }
    public string? InitialDateNotes { get; set; }
    public string? FinalDateNotes { get; set; }
    public string? Observations { get; set; }
}
