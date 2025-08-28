namespace CatalogingSystem.DTOs.Dtos;

public class TenantCustomizationDto
{
    public required string Header { get; set; }
    public required string Background { get; set; }
    public required string Steps { get; set; }
    public required string SelectedSteps { get; set; }
    public required string PrimaryButton { get; set; }
    public required string SecondaryButton { get; set; }
    public required string Admin { get; set; }
    public required string Modification { get; set; }
    public required string ReadOnly { get; set; }
    public required string Director { get; set; }
    public required string Researcher { get; set; }
}
