using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CatalogingSystem.Data.DbContext;

public partial class ApplicationDbContext : IdentityDbContext<User>
{
    private readonly ICurrentTenantService _tenantService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentTenantService tenantService,
        IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _tenantService = tenantService;
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<ArchivoAdministrativo> ArchivosAdministrativos { get; set; }
    public DbSet<Identification> Identifications { get; set; }
    public DbSet<GraphicDocumentation> GraphicDocumentations { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var auditEntries = OnBeforeSaveChanges();
        int result = await base.SaveChangesAsync(cancellationToken);
        if (auditEntries.Any())
        {
            await OnAfterSaveChanges(auditEntries);
        }
        return result;
    }

    private List<AuditEntry> OnBeforeSaveChanges()
    {
        var auditEntries = new List<AuditEntry>();
        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                var auditEntry = new AuditEntry
                {
                    TenantId = _tenantService.TenantId ?? throw new InvalidOperationException("TenantId no está configurado."),
                    EntityName = entry.Entity.GetType().Name,
                    EntityExpediente = entry.Entity.expediente,
                    Action = entry.State == EntityState.Added ? "Create" : "Update",
                    Username = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown",
                    UserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    Timestamp = DateTime.UtcNow
                };

                if (entry.State == EntityState.Modified)
                {
                    auditEntry.Changes = GetChanges(entry);
                }

                auditEntries.Add(auditEntry);
            }
        }
        return auditEntries;
    }

    private async Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
    {
        foreach (var auditEntry in auditEntries)
        {
            AuditLogs.Add(new AuditLog
            {
                Id = Guid.NewGuid(),
                TenantId = auditEntry.TenantId,
                EntityName = auditEntry.EntityName,
                EntityExpediente = auditEntry.EntityExpediente,
                Action = auditEntry.Action,
                UserId = auditEntry.UserId,
                Username = auditEntry.Username ?? "Unknown",
                Timestamp = auditEntry.Timestamp,
                Changes = auditEntry.Changes
            });
        }
        await base.SaveChangesAsync();
    }

    private string GetChanges(EntityEntry entry)
    {
        var changes = new Dictionary<string, object>();
        var propertiesToExclude = new HashSet<string> { "Password" };
        foreach (var property in entry.OriginalValues.Properties)
        {
            if (propertiesToExclude.Contains(property.Name)) continue;
            var originalValue = entry.OriginalValues[property];
            var currentValue = entry.CurrentValues[property];
            if (!Equals(originalValue, currentValue))
            {
                changes[property.Name] = new { OldValue = originalValue, NewValue = currentValue };
            }
        }
        return JsonSerializer.Serialize(changes);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!string.IsNullOrEmpty(_tenantService?.ConnectionString))
        {
            optionsBuilder.UseNpgsql(_tenantService.ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ArchivoAdministrativo configuration
        modelBuilder.Entity<ArchivoAdministrativo>()
            .Property(a => a.Id).HasDefaultValueSql("gen_random_uuid()");

        modelBuilder.Entity<ArchivoAdministrativo>()
            .Property(a => a.institucion)
            .HasConversion<string>();

        modelBuilder.Entity<ArchivoAdministrativo>()
            .Property(a => a.documentoOrigen)
            .HasConversion<string>();

        modelBuilder.Entity<ArchivoAdministrativo>()
            .HasIndex(a => a.expediente)
            .IsUnique();

        // Identification configuration
        modelBuilder.Entity<Identification>()
            .Property(i => i.Id).HasDefaultValueSql("gen_random_uuid()");

        modelBuilder.Entity<Identification>()
            .HasOne(i => i.ArchivoAdministrativo)
            .WithMany()
            .HasForeignKey(i => i.expediente)
            .HasPrincipalKey(a => a.expediente)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Identification>()
            .OwnsOne(i => i.section);

        modelBuilder.Entity<Identification>()
            .OwnsOne(i => i.typology);

        modelBuilder.Entity<Identification>()
            .OwnsOne(i => i.specificName);

        modelBuilder.Entity<Identification>()
            .OwnsOne(i => i.author);

        modelBuilder.Entity<Identification>()
            .OwnsOne(i => i.title);

        modelBuilder.Entity<Identification>()
            .OwnsOne(i => i.material);

        modelBuilder.Entity<Identification>()
            .OwnsOne(i => i.techniques);
        
        // GraphicDocumentation configuration
        modelBuilder.Entity<GraphicDocumentation>()
            .Property(g => g.Id).HasDefaultValueSql("gen_random_uuid()");

        modelBuilder.Entity<GraphicDocumentation>()
            .HasOne(g => g.ArchivoAdministrativo)
            .WithOne()
            .HasForeignKey<GraphicDocumentation>(g => g.expediente)
            .HasPrincipalKey<ArchivoAdministrativo>(a => a.expediente)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GraphicDocumentation>()
            .OwnsOne(g => g.dimensions);

        modelBuilder.Entity<GraphicDocumentation>()
            .OwnsOne(g => g.imageAuthor);

        modelBuilder.Entity<GraphicDocumentation>()
            .HasIndex(g => g.expediente)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.TenantId)
            .IsRequired(false);

        // Configuración de AuditLog
        modelBuilder.Entity<AuditLog>()
            .Property(a => a.Id).HasDefaultValueSql("gen_random_uuid()");
        modelBuilder.Entity<AuditLog>()
            .Property(a => a.TenantId).IsRequired();
        modelBuilder.Entity<AuditLog>()
            .Property(a => a.EntityName).IsRequired();
        modelBuilder.Entity<AuditLog>()
            .Property(a => a.EntityExpediente).IsRequired();
        modelBuilder.Entity<AuditLog>()
            .Property(a => a.Action).IsRequired();
        modelBuilder.Entity<AuditLog>()
            .Property(a => a.Username).IsRequired();
        modelBuilder.Entity<AuditLog>()
            .Property(a => a.Timestamp).IsRequired();
        modelBuilder.Entity<AuditLog>()
            .Property(a => a.Changes).HasColumnType("jsonb").IsRequired(false);

        // Índices para búsquedas rápidas
        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => a.TenantId);
        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => a.EntityExpediente);
        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => a.Timestamp);
        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => a.Username);
    }
}

