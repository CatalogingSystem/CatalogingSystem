namespace CatalogingSystem.Core.Entities;

public class SimpleDate
{
    public bool IsPresent { get; set; } = true;
    public string? Exact { get; set; }
    public string? Approximate { get; set; }
    public string? Probable { get; set; }
    public long? BC { get; set; }
    public long? Year { get; set; }
    public long? Month { get; set; }
    public long? Day { get; set; }
}
