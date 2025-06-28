namespace CatalogingSystem.DTOs.Dtos;

public class AuditLogDto
{
    public string Action { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Changes { get; set; }
}