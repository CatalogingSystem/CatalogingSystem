using AutoMapper;
using CatalogingSystem.Core.Entities.DescriptionClassification;
using CatalogingSystem.DTOs.Dtos;

namespace CatalogingSystem.DTOs.Mapping;

public class DescriptionClassificationProfile : Profile
{
    public DescriptionClassificationProfile()
    {
        CreateMap<DescriptionClassificationDto, DescriptionClassification>();
        CreateMap<DescriptionClassification, DescriptionClassificationDto>();
        CreateMap<UpdateDescriptionClassificationDto, DescriptionClassification>();
        CreateMap<DecorationDto, Decoration>();
        CreateMap<Decoration, DecorationDto>();
        CreateMap<TechnicalCharacteristicsDto, TechnicalCharacteristics>();
        CreateMap<TechnicalCharacteristics, TechnicalCharacteristicsDto>();
        CreateMap<DescriptionDimensionsDto, DescriptionDimensions>();
        CreateMap<DescriptionDimensions, DescriptionDimensionsDto>();
        CreateMap<SignaturesAndMarksDto, SignaturesAndMarks>();
        CreateMap<SignaturesAndMarks, SignaturesAndMarksDto>();
        CreateMap<InscriptionsDto, Inscriptions>();
        CreateMap<Inscriptions, InscriptionsDto>();
        CreateMap<PlaceOfElaborationDto, PlaceOfElaboration>();
        CreateMap<PlaceOfElaboration, PlaceOfElaborationDto>();
        CreateMap<CulturalContextDto, CulturalContext>();
        CreateMap<CulturalContext, CulturalContextDto>();
        CreateMap<CollectionProvenanceDto, CollectionProvenance>();
        CreateMap<CollectionProvenance, CollectionProvenanceDto>();
        CreateMap<ReasonedClassificationDto, ReasonedClassification>();
        CreateMap<ReasonedClassification, ReasonedClassificationDto>();
        CreateMap<BibliographyDto, Bibliography>();
        CreateMap<Bibliography, BibliographyDto>();
        CreateMap<ObjectHistoryDto, ObjectHistory>();
        CreateMap<ObjectHistory, ObjectHistoryDto>();
    }
}
