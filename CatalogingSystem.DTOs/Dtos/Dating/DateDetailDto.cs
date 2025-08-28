namespace CatalogingSystem.DTOs.Dtos;

public class DateDetailDto
{
    public string? Exact { get; set; }
    public string? Approximate { get; set; }
    public string? Probable { get; set; }
    public long? BC { get; set; }
    public long? Year { get; set; }
    public long? Month { get; set; }
    public long? Day { get; set; }
}
