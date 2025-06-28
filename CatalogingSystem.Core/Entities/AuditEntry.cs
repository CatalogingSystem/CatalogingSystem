namespace CatalogingSystem.Core.Entities;
public class AuditEntry
{
    public required string TenantId { get; set; }
    public string? EntityName { get; set; }
    public long EntityExpediente { get; set; }
    public required string Action { get; set; }
    public string? UserId { get; set; }
    public string? Username { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Changes { get; set; }
}