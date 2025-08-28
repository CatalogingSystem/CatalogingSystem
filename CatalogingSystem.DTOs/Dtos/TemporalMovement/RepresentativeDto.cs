namespace CatalogingSystem.DTOs.Dtos;

public class RepresentativeDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string IdentityCard { get; set; }
    public required string InstitutionalId { get; set; }
    public required string Institution { get; set; }
    public required string Address { get; set; }
    public required string Locality { get; set; }
    public required string Province { get; set; }
    public required string Department { get; set; }
    public required string Country { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public string? References { get; set; }
    public string? Observations { get; set; }
}
