namespace CatalogingSystem.DTOs.Dtos;

public class MetricResult<T>
{
    public int Total { get; set; }
    public IEnumerable<MetricItem<T>> Items { get; set; } = new List<MetricItem<T>>();
}
