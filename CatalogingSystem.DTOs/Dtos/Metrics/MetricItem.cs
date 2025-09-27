namespace CatalogingSystem.DTOs.Dtos;

public class MetricItem<T>
{
    public T Key { get; set; } = default!;
    public int Count { get; set; }
    public double Percentage { get; set; }
}
