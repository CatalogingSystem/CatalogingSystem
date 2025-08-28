namespace CatalogingSystem.DTOs.Dtos;

public class UpdateDatingDto
{
    public SimpleDateDto? SimpleDate { get; set; }
    public DateRangeDto? DateRange { get; set; }
    public ApproximateDatingDto? ApproximateDating { get; set; }
    public DatingNotesDto? Notes { get; set; }
}
