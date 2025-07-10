using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs;
using CatalogingSystem.DTOs.Dtos;

namespace CatalogingSystem.Services.Interfaces;

public interface IUserService
{
    Task<User> CreateUserAsync(CreateUserRequestDto request);
    Task<PagedResultDto<UserDto>> GetUsersAsync(int page = 1, int size = 10);
    Task<bool> UpdateUserAsync(string userId, UpdateUserRequestDto request);
}