namespace CatalogingSystem.Core.Entities;

public class DateRange
{
    public bool IsPresent { get; set; } = true;
    public DateDetail? From { get; set; }
    public DateDetail? To { get; set; }
}
