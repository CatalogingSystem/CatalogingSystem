namespace CatalogingSystem.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string? PermissionLevel { get; set; }
}