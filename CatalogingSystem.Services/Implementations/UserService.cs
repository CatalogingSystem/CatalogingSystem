using CatalogingSystem.Core.Entities;
using CatalogingSystem.Core.Enums;
using CatalogingSystem.Core.Interfaces;
using CatalogingSystem.Data.DbContext;
using CatalogingSystem.DTOs;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CatalogingSystem.Services.Implementations;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly ICurrentTenantService _tenantService;
    private readonly BaseDbContext _baseDbContext;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        ICurrentTenantService tenantService,
        BaseDbContext baseDbContext
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tenantService = tenantService;
        _baseDbContext = baseDbContext;
    }

    public async Task<User> CreateUserAsync(CreateUserRequestDto request)
    {
        if (request.Role.Equals("SuperDirector", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "No se permite crear usuarios con el rol 'SuperDirector'."
            );
        }

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            throw new InvalidOperationException(
                $"Rol inválido: {request.Role}. Solo se permiten 'Director' e 'Investigador'."
            );
        }

        InvestigatorPermissionLevel? permissionLevel = null;
        if (role == UserRole.Investigador)
        {
            if (string.IsNullOrEmpty(request.PermissionLevel))
            {
                throw new InvalidOperationException(
                    "El nivel de permisos es requerido para el rol Investigador."
                );
            }
            if (
                !Enum.TryParse<InvestigatorPermissionLevel>(
                    request.PermissionLevel,
                    true,
                    out var parsedPermissionLevel
                )
            )
            {
                throw new InvalidOperationException(
                    $"Nivel de permisos inválido: {request.PermissionLevel}. Solo se permiten 'ReadOnly' o 'ReadWrite'."
                );
            }
            permissionLevel = parsedPermissionLevel;
        }
        else if (!string.IsNullOrEmpty(request.PermissionLevel))
        {
            throw new InvalidOperationException(
                "El nivel de permisos solo se aplica al rol Investigador."
            );
        }

        var user = new User
        {
            UserName = request.Username,
            TenantId = request.TenantId,
            PermissionLevel = permissionLevel,
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Error al crear usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}"
            );
        }

        // Crear el rol si no existe
        if (!await _roleManager.RoleExistsAsync(request.Role))
        {
            await _roleManager.CreateAsync(new IdentityRole(request.Role));
        }

        await _userManager.AddToRoleAsync(user, request.Role);

        return user;
    }

    public async Task<PagedResultDto<UserDto>> GetUsersAsync(int page = 1, int size = 10)
    {
        if (string.IsNullOrEmpty(_tenantService.TenantId))
        {
            throw new InvalidOperationException("No tenant context available.");
        }
        if (page < 1)
            page = 1;
        if (page < 1)
            page = 10;

        var query = _userManager.Users.Where(u => u.TenantId == _tenantService.TenantId);

        int totalItems = await query.CountAsync();

        var users = await query
            .OrderBy(u => u.UserName)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            userDtos.Add(
                new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    TenantId = user.TenantId,
                    Role = role,
                    PermissionLevel =
                        role == "Investigador" ? user.PermissionLevel?.ToString() : null,
                }
            );
        }

        return new PagedResultDto<UserDto>
        {
            Items = userDtos,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)size),
            CurrentPage = page,
            PageSize = size,
        };
    }

    public async Task<bool> UpdateUserAsync(string userId, UpdateUserRequestDto request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }
        if (!string.IsNullOrEmpty(request.Username))
        {
            user.UserName = request.Username;
        }
        if (!string.IsNullOrEmpty(request.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.Password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Error al actualizar la contraseña.");
            }
        }
        if (!string.IsNullOrEmpty(request.Role))
        {
            if (request.Role.Equals("SuperDirector", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "No se permite asignar el rol 'SuperDirector'."
                );
            }
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, request.Role);
        }
        if (!string.IsNullOrEmpty(request.PermissionLevel))
        {
            if (
                Enum.TryParse<InvestigatorPermissionLevel>(
                    request.PermissionLevel,
                    true,
                    out var permissionLevel
                )
            )
            {
                user.PermissionLevel = permissionLevel;
            }
            else
            {
                throw new InvalidOperationException("Nivel de permisos inválido.");
            }
        }
        var updateResult = await _userManager.UpdateAsync(user);
        return updateResult.Succeeded;
    }
}
