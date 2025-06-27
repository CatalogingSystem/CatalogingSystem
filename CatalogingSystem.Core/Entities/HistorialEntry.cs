using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogingSystem.Core.Entities;

public class HistorialEntry
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid AuditLogId { get; set; }

    [ForeignKey("AuditLogId")]
    public AuditLog? AuditLog { get; set; }
    
}