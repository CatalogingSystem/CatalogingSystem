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
            if (oldData == null && newData == null)
            {
                return;
            }

            if (oldData != null && newData != null)
            {
                var changedProperties = new List<object>();
                CompareProperties(oldData, newData, "", changedProperties);

                if (changedProperties.Any())
                {
                    var oldValues = changedProperties.Select(p => new
                    {
                        Property = p.GetType().GetProperty("Property")?.GetValue(p),
                        OldValue = p.GetType().GetProperty("OldValue")?.GetValue(p)
                    }).ToList();
                    var newValues = changedProperties.Select(p => new
                    {
                        Property = p.GetType().GetProperty("Property")?.GetValue(p),
                        NewValue = p.GetType().GetProperty("NewValue")?.GetValue(p)
                    }).ToList();

                    var serializerOptions = new JsonSerializerOptions
                    {
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };

                    oldDataJson = JsonSerializer.Serialize(oldValues, serializerOptions);
                    newDataJson = JsonSerializer.Serialize(newValues, serializerOptions);
                }
                else
                {
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
            return;
        }

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

    private string GetFriendlyTypeName(Type type)
    {
        if (type.IsGenericType)
        {
            if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                return GetFriendlyTypeName(underlyingType) + "?";
            }
            else
            {
                var genericArgs = type.GetGenericArguments();
                var typeNames = string.Join(", ", genericArgs.Select(t => GetFriendlyTypeName(t)));
                return $"{type.Name.Split('`')[0]}<{typeNames}>";
            }
        }
        return type.Name;
    }

    private void CompareProperties(object oldData, object newData, string prefix, List<object> changedProperties)
    {
        if (oldData == null || newData == null) return;

        var type = oldData.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (prop.Name == "Id" || !prop.CanRead) continue;

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
                    changedProperties.Add(new
                    {
                        Property = $"{prefix}{prop.Name}",
                        OldValue = oldValue != null ? new { Value = oldValue, Type = GetFriendlyTypeName(oldValue.GetType()) } : null,
                        NewValue = newValue != null ? new { Value = newValue, Type = GetFriendlyTypeName(newValue.GetType()) } : null
                    });
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
            {
                var oldCollection = oldValue as IEnumerable;
                var newCollection = newValue as IEnumerable;
                if (!AreCollectionsEqual(oldCollection, newCollection))
                {
                    changedProperties.Add(new
                    {
                        Property = $"{prefix}{prop.Name}",
                        OldValue = oldCollection != null ? new { Value = oldCollection, Type = GetFriendlyTypeName(prop.PropertyType) } : null,
                        NewValue = newCollection != null ? new { Value = newCollection, Type = GetFriendlyTypeName(prop.PropertyType) } : null
                    });
                }
            }
            else
            {
                if (!Equals(oldValue, newValue))
                {
                    changedProperties.Add(new
                    {
                        Property = $"{prefix}{prop.Name}",
                        OldValue = oldValue != null ? new { Value = oldValue, Type = GetFriendlyTypeName(prop.PropertyType) } : null,
                        NewValue = newValue != null ? new { Value = newValue, Type = GetFriendlyTypeName(prop.PropertyType) } : null
                    });
                }
            }
        }
    }

    private bool AreCollectionsEqual(IEnumerable oldCollection, IEnumerable newCollection)
    {
        if (oldCollection == null && newCollection == null) return true;
        if (oldCollection == null || newCollection == null) return false;
        return oldCollection.Cast<object>().SequenceEqual(newCollection.Cast<object>());
    }
}