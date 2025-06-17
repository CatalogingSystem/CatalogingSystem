using CatalogingSystem.DTOs;

namespace CatalogingSystem.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request);
}