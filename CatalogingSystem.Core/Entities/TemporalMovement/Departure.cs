namespace CatalogingSystem.Core.Entities;

public class Departure
{
    public string? Company { get; set; }
    public string? Location { get; set; }
    public DateTime? Date { get; set; }
    public string? Time { get; set; }
    public string? Notes { get; set; }
}