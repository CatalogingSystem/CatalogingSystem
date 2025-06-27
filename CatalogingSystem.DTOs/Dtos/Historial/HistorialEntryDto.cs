namespace CatalogingSystem.DTOs.Dtos;

public class HistorialEntryDto
{
    public DateTime Timestamp { get; set; }
    public required string Username { get; set; }
    public required string Action { get; set; }
    public required string Details { get; set; }
}