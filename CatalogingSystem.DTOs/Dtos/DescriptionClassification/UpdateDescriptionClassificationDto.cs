namespace CatalogingSystem.DTOs.Dtos;

public class UpdateDescriptionClassificationDto
{
    public string? Description { get; set; }
    public DecorationDto? Decoration { get; set; }
    public TechnicalCharacteristicsDto? TechnicalCharacteristics { get; set; }
    public DescriptionDimensionsDto? DescriptionDimensions { get; set; }
    public SignaturesAndMarksDto? SignaturesAndMarks { get; set; }
    public InscriptionsDto? Inscriptions { get; set; }
    public PlaceOfElaborationDto? PlaceOfElaboration { get; set; }
    public CulturalContextDto? CulturalContext { get; set; }
    public CollectionProvenanceDto? CollectionProvenance { get; set; }
    public ReasonedClassificationDto? ReasonedClassification { get; set; }
    public BibliographyDto? Bibliography { get; set; }
    public ObjectHistoryDto? ObjectHistory { get; set; }
    public string? Observations { get; set; }
} 