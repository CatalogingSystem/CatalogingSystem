using System.ComponentModel.DataAnnotations;

namespace CatalogingSystem.Core.Entities;

public class AuditLog
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public required string EntityType { get; set; }

    [Required]
    public required string EntityId { get; set; }

    [Required]
    public required string Action { get; set; }

    [Required]
    public required string UserId { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    public required string TenantId { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }

    public string? Details { get; set; }
}