using System;
using CatalogingSystem.Core.Enums;

namespace CatalogingSystem.Core.Entities;

public class Dating
{
    public Guid Id { get; set; }
    public required long Expediente { get; set; }
    public required long Inventory { get; set; }
    public SimpleDate? SimpleDate { get; set; }
    public DateRange? DateRange { get; set; }
    public ApproximateDating? ApproximateDating { get; set; }
    public DatingNotes? Notes { get; set; }
    public ArchivoAdministrativo ArchivoAdministrativo { get; set; }
}
