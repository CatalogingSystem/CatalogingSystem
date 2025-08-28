using CatalogingSystem.Core.Enums;

namespace CatalogingSystem.DTOs.Dtos;

public class ApproximateDatingDto
{
    public PeriodEnum? FromCentury { get; set; }
    public PeriodEnum? ToCentury { get; set; }
}
