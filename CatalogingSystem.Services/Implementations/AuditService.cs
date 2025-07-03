namespace CatalogingSystem.Services.Implementations;

using CatalogingSystem.Core.Entities;
using CatalogingSystem.Core.Interfaces;
using CatalogingSystem.Data.DbContext;
using CatalogingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentTenantService _tenantService;

    public AuditService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ICurrentTenantService tenantService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _tenantService = tenantService;
    }

    public async Task LogAuditAsync<T>(string operation, Guid? recordId, T? oldData, T? newData, string? context = null)
    {
        string? oldDataJson = null;
        string? newDataJson = null;

        if (operation == "UPDATE")
        {
            // Si ambos oldData y newData son null, no crear auditoría
            if (oldData == null && newData == null)
            {
                return;
            }

            // Si hay datos para comparar, verificar si hay cambios
            if (oldData != null && newData != null)
            {
                var changedProperties = new Dictionary<string, (string OldValue, string NewValue)>();
                CompareProperties(oldData, newData, "", changedProperties);

                if (changedProperties.Any())
                {
                    oldDataJson = JsonSerializer.Serialize(changedProperties.Select(p => new { Property = p.Key, OldValue = p.Value.OldValue }));
                    newDataJson = JsonSerializer.Serialize(changedProperties.Select(p => new { Property = p.Key, NewValue = p.Value.NewValue }));
                }
                else
                {
                    // No hay cambios, no crear auditoría
                    return;
                }
            }
        }
        else if (operation == "CREATE" || operation == "DELETE")
        {
            oldDataJson = oldData != null ? JsonSerializer.Serialize(oldData) : null;
            newDataJson = newData != null ? JsonSerializer.Serialize(newData) : null;
        }
        else
        {
            // Para otras operaciones, no hacer nada por ahora
            return;
        }

        // Crear el registro de auditoría solo si hay datos relevantes
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            TableName = typeof(T).Name,
            Operation = operation,
            RecordId = recordId,
            Expediente = (newData ?? oldData) != null ? GetExpedienteFromObject((newData ?? oldData)!) : null,
            OldData = oldDataJson,
            NewData = newDataJson,
            UserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Username = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown",
            TenantId = _tenantService.TenantId,
            ActionTimestamp = DateTime.UtcNow,
            Context = context ?? _httpContextAccessor.HttpContext?.Request.Path
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }

    private long? GetExpedienteFromObject(object obj)
    {
        if (obj == null) return null;

        var type = obj.GetType();
        var prop = type.GetProperty("expediente", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop != null && prop.PropertyType == typeof(long))
        {
            return (long?)prop.GetValue(obj);
        }
        return null;
    }

    private void CompareProperties(object oldData, object newData, string prefix, Dictionary<string, (string OldValue, string NewValue)> changedProperties)
    {
        if (oldData == null || newData == null) return;

        var type = oldData.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (prop.Name == "Id" || !prop.CanRead) continue; // Ignorar propiedades sin getter

            var oldValue = prop.GetValue(oldData);
            var newValue = prop.GetValue(newData);

            if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string) && !prop.PropertyType.IsArray && !typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
            {
                if (oldValue != null && newValue != null)
                {
                    CompareProperties(oldValue, newValue, $"{prefix}{prop.Name}.", changedProperties);
                }
                else if (oldValue != newValue)
                {
                    changedProperties[$"{prefix}{prop.Name}"] = (oldValue?.ToString() ?? "null", newValue?.ToString() ?? "null");
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
            {
                // Manejar colecciones como List<string>
                var oldCollection = oldValue as IEnumerable;
                var newCollection = newValue as IEnumerable;
                string oldCollectionStr = oldCollection != null ? string.Join(", ", oldCollection.Cast<object>()) : "null";
                string newCollectionStr = newCollection != null ? string.Join(", ", newCollection.Cast<object>()) : "null";
                if (oldCollectionStr != newCollectionStr)
                {
                    changedProperties[$"{prefix}{prop.Name}"] = (oldCollectionStr, newCollectionStr);
                }
            }
            else
            {
                var oldValueStr = oldValue?.ToString();
                var newValueStr = newValue?.ToString();

                if (oldValueStr != newValueStr)
                {
                    changedProperties[$"{prefix}{prop.Name}"] = (oldValueStr ?? "null", newValueStr ?? "null");
                }
            }
        }
    }
}