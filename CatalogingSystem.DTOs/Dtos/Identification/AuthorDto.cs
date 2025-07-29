namespace CatalogingSystem.DTOs.Dtos;

public class AuthorDto
{
    public string Name { get; set; } = string.Empty;
    public string BirthPlace { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string DeathPlace { get; set; } = string.Empty;
    public DateTime DeathDate { get; set; }
}