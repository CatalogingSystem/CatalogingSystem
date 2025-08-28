using CatalogingSystem.Core.Enums;

namespace CatalogingSystem.Core.Entities;

public class TemporalMovement
{
    public Guid Id { get; set; }
    public required MovementType MovementType { get; set; }
    public required long Expediente { get; set; }
    public required Applicant Applicant { get; set; }
    public Representative? Representative { get; set; }
    public string? Entity { get; set; }
    public required string TransferLocation { get; set; }
    public required DateTime DepartureDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string? Document { get; set; }
    public string? Code { get; set; }
    public DateTime? Date { get; set; }
    public string? Insurer { get; set; }
    public string? Policy { get; set; }
    public string? Notes { get; set; }
    public Departure? Departure { get; set; }
    public Return? Return { get; set; }
    public string? Observations { get; set; }
    public ArchivoAdministrativo ArchivoAdministrativo { get; set; }
}
