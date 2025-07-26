namespace CatalogingSystem.DTOs.Dtos;

public class CatalogItemDto
{
    public long Expediente { get; set; }
    public ArchivoAdministrativoDto ArchivoAdministrativo { get; set; }
    public IdentificationDto? Identification { get; set; }
    public GraphicDocumentationDto? GraphicDocumentation { get; set; }
    public AdministrativeDataDto? AdministrativeData { get; set; }
    public ConservationDto? Conservation { get; set; }
    public DatingDto? Dating { get; set; }
}