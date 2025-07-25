using CatalogingSystem.Core.Enums;

namespace CatalogingSystem.Core.Entities;

public class ApproximateDating
{
    public bool IsPresent { get; set; } = true;
    public PeriodEnum? FromCentury { get; set; }
    public PeriodEnum? ToCentury { get; set; }
}