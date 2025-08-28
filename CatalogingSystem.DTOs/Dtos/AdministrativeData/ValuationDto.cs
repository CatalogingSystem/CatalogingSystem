using System;

namespace CatalogingSystem.DTOs.Dtos;

public class ValuationDto
{
    public string? Value { get; set; }
    public string? Appraiser { get; set; }
    public DateTime? Date { get; set; }
    public string? Notes { get; set; }
}
