using CatalogingSystem.Core.Enums;

namespace CatalogingSystem.DTOs.Dtos;

public class TemporalMovementDto
{
    public required MovementType MovementType { get; set; }
    public required long Expediente { get; set; }
    public required ApplicantDto Applicant { get; set; }
    public RepresentativeDto? Representative { get; set; }
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
    public DepartureDto? Departure { get; set; }
    public ReturnDto? Return { get; set; }
    public string? Observations { get; set; }
}