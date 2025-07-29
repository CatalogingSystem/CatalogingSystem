namespace CatalogingSystem.DTOs;

public class AuditLogDto
{
    public Guid Id { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public Guid? RecordId { get; set; }
    public long? Expediente { get; set; }
    public string? OldData { get; set; }
    public string? NewData { get; set; }
    public string? UserId { get; set; }
    public string? Username { get; set; }
    public string? TenantId { get; set; }
    public DateTime ActionTimestamp { get; set; }
    public string? Context { get; set; }
}