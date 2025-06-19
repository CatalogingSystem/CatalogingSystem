namespace CatalogingSystem.DTOs.Dtos;

public class TenantDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string ISIL { get; set; }
    public string? Description { get; set; }
}