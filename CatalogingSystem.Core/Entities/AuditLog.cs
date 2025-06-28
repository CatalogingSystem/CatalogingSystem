namespace CatalogingSystem.Core.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public required string TenantId { get; set; }
    public required string EntityName { get; set; }
    public required long EntityExpediente { get; set; }
    public required string Action { get; set; }
    public string? UserId { get; set; }
    public required string Username { get; set; }
    public required DateTime Timestamp { get; set; }
    public string? Changes { get; set; }
}