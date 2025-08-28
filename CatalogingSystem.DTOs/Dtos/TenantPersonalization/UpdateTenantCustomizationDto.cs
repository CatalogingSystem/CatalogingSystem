namespace CatalogingSystem.DTOs.Dtos;

public class UpdateTenantCustomizationDto
{
    public string? Header { get; set; }
    public string? Background { get; set; }
    public string? Steps { get; set; }
    public string? SelectedSteps { get; set; }
    public string? PrimaryButton { get; set; }
    public string? SecondaryButton { get; set; }
    public string? Admin { get; set; }
    public string? Modification { get; set; }
    public string? ReadOnly { get; set; }
    public string? Director { get; set; }
    public string? Researcher { get; set; }
}
