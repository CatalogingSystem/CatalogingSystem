namespace CatalogingSystem.Core.Entities;

public class Return
{   
    public bool IsPresent { get; set; } = true;
    public string? Company { get; set; }
    public string? Location { get; set; }
    public DateTime? Date { get; set; }
    public string? Time { get; set; }
    public string? Notes { get; set; }
}