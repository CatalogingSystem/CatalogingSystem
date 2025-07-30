using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.Core.Interfaces;
using CatalogingSystem.Core.Entities.DescriptionClassification;

namespace CatalogingSystem.Data.DbContext;

public partial class ApplicationDbContext : IdentityDbContext<User>
{
    private readonly ICurrentTenantService _tenantService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentTenantService tenantService) 
        : base(options)
    {
        _tenantService = tenantService;
    }

    public DbSet<ArchivoAdministrativo> ArchivosAdministrativos { get; set; }
    public DbSet<Identification> Identifications { get; set; }
    public DbSet<GraphicDocumentation> GraphicDocumentations { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<AdministrativeData> AdministrativeData { get; set; }
    public DbSet<TemporalMovement> TemporalMovements { get; set; }
    public DbSet<Dating> Datings { get; set; }
    public DbSet<Conservation> Conservations { get; set; }
    public DbSet<DescriptionClassification> DescriptionClassifications { get; set; }
    public DbSet<TenantCustomization> TenantCustomizations { get; set; }

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

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>()
            .Property(a => a.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        // AdministrativeData configuration
        modelBuilder.Entity<AdministrativeData>()
            .Property(ad => ad.Id).HasDefaultValueSql("gen_random_uuid()");

        modelBuilder.Entity<AdministrativeData>()
            .Property(ad => ad.EntryForm)
            .HasConversion<string>();

        modelBuilder.Entity<AdministrativeData>()
            .HasOne(ad => ad.ArchivoAdministrativo)
            .WithOne()
            .HasForeignKey<AdministrativeData>(ad => ad.FileNumber)
            .HasPrincipalKey<ArchivoAdministrativo>(a => a.expediente)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AdministrativeData>()
            .OwnsOne(ad => ad.CopiesReproductions);

        modelBuilder.Entity<AdministrativeData>()
            .OwnsOne(ad => ad.Valuation);

        modelBuilder.Entity<AdministrativeData>()
            .OwnsOne(ad => ad.Cataloger);

        modelBuilder.Entity<AdministrativeData>()
            .HasIndex(ad => ad.FileNumber)
            .IsUnique();

        // TemporalMovement configuration
        modelBuilder.Entity<TemporalMovement>()
            .Property(tm => tm.Id).HasDefaultValueSql("gen_random_uuid()");

        modelBuilder.Entity<TemporalMovement>()
            .HasOne(tm => tm.ArchivoAdministrativo)
            .WithMany()
            .HasForeignKey(tm => tm.Expediente)
            .HasPrincipalKey(a => a.expediente)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TemporalMovement>()
            .OwnsOne(tm => tm.Applicant);

        modelBuilder.Entity<TemporalMovement>()
            .OwnsOne(tm => tm.Representative);

        modelBuilder.Entity<TemporalMovement>()
            .OwnsOne(tm => tm.Departure);

        modelBuilder.Entity<TemporalMovement>()
            .OwnsOne(tm => tm.Return);
            
        // Dating configuration
        modelBuilder.Entity<Dating>()
            .Property(d => d.Id).HasDefaultValueSql("gen_random_uuid()");

        modelBuilder.Entity<Dating>()
            .HasOne(d => d.ArchivoAdministrativo)
            .WithOne()
            .HasForeignKey<Dating>(d => d.Expediente)
            .HasPrincipalKey<ArchivoAdministrativo>(a => a.expediente)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Dating>()
            .OwnsOne(d => d.SimpleDate);

        modelBuilder.Entity<Dating>()
            .OwnsOne(d => d.DateRange, dr =>
            {
                dr.OwnsOne(r => r.From);
                dr.OwnsOne(r => r.To);
            });

        modelBuilder.Entity<Dating>()
            .OwnsOne(d => d.ApproximateDating);

        modelBuilder.Entity<Dating>()
            .OwnsOne(d => d.Notes);

        modelBuilder.Entity<Dating>()
            .HasIndex(d => d.Expediente)
            .IsUnique();
        // Conservation configuration
        modelBuilder.Entity<Conservation>()
            .Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");

        modelBuilder.Entity<Conservation>()
            .HasOne(c => c.ArchivoAdministrativo)
            .WithOne()
            .HasForeignKey<Conservation>(c => c.Expediente)
            .HasPrincipalKey<ArchivoAdministrativo>(a => a.expediente)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Conservation>()
            .HasIndex(c => c.Expediente)
            .IsUnique();
        
        // DescriptionClassification configuration
        modelBuilder.Entity<DescriptionClassification>()
            .Property(dc => dc.Id).HasDefaultValueSql("gen_random_uuid()");

        modelBuilder.Entity<DescriptionClassification>()
            .HasOne(dc => dc.ArchivoAdministrativo)
            .WithOne()
            .HasForeignKey<DescriptionClassification>(dc => dc.Expediente)
            .HasPrincipalKey<ArchivoAdministrativo>(a => a.expediente)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DescriptionClassification>()
            .OwnsOne(dc => dc.Decoration);

        modelBuilder.Entity<DescriptionClassification>()
            .OwnsOne(dc => dc.TechnicalCharacteristics);

        modelBuilder.Entity<DescriptionClassification>()
            .OwnsOne(dc => dc.DescriptionDimensions);

        modelBuilder.Entity<DescriptionClassification>()
            .OwnsOne(dc => dc.SignaturesAndMarks);

        modelBuilder.Entity<DescriptionClassification>()
            .OwnsOne(dc => dc.Inscriptions);

        modelBuilder.Entity<DescriptionClassification>()
            .OwnsOne(dc => dc.PlaceOfElaboration);

        modelBuilder.Entity<DescriptionClassification>()
            .OwnsOne(dc => dc.CulturalContext);

        modelBuilder.Entity<DescriptionClassification>()
            .OwnsOne(dc => dc.CollectionProvenance);

        modelBuilder.Entity<DescriptionClassification>()
            .OwnsOne(dc => dc.ReasonedClassification);

        modelBuilder.Entity<DescriptionClassification>()
            .OwnsOne(dc => dc.Bibliography);

        modelBuilder.Entity<DescriptionClassification>()
            .OwnsOne(dc => dc.ObjectHistory);

        modelBuilder.Entity<DescriptionClassification>()
            .HasIndex(dc => dc.Expediente)
            .IsUnique();
    }
}