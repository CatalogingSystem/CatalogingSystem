using System.Text.Json.Serialization;

namespace CatalogingSystem.Core.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MovementType
{
    Restoration,
    Conservation,
    InternalTemporaryExhibition,
    ExternalTemporaryExhibition,
    LaboratoryAnalysis,
    PhotographicRecord,
    Studies,
    PermanentExhibitionSiteChange,
    Relocation,
    Loan,
    Disposal
}