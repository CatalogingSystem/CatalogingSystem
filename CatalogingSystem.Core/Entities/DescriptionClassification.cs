namespace CatalogingSystem.Core.Entities.DescriptionClassification;

public class DescriptionClassification
{
    public Guid Id { get; set; }
    public required long Expediente { get; set; }
    public string? Description { get; set; }
    public Decoration? Decoration { get; set; }
    public TechnicalCharacteristics? TechnicalCharacteristics { get; set; }
    public DescriptionDimensions? DescriptionDimensions { get; set; }
    public SignaturesAndMarks? SignaturesAndMarks { get; set; }
    public Inscriptions? Inscriptions { get; set; }
    public PlaceOfElaboration? PlaceOfElaboration { get; set; }
    public CulturalContext? CulturalContext { get; set; }
    public CollectionProvenance? CollectionProvenance { get; set; }
    public ReasonedClassification? ReasonedClassification { get; set; }
    public Bibliography? Bibliography { get; set; }
    public ObjectHistory? ObjectHistory { get; set; }
    public string? Observations { get; set; }
    public ArchivoAdministrativo ArchivoAdministrativo { get; set; }
}
