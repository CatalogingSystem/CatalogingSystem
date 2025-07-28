namespace CatalogingSystem.DTOs.Mapping;

using AutoMapper;
using CatalogingSystem.DTOs.Dtos;

public class CatalogItemProfile : Profile
{
    public CatalogItemProfile()
    {
        CreateMap<CatalogItemDto, CatalogItemDto>();
        CreateMap<IdentificationDto, UpdateIdentificationDto>();
        CreateMap<DescriptionClassificationDto, UpdateDescriptionClassificationDto>();
        CreateMap<AdministrativeDataDto, UpdateAdministrativeDataDto>();
        CreateMap<ConservationDto, UpdateConservationDto>();
        CreateMap<GraphicDocumentationDto, UpdateGraphicDocumentationDto>();
        CreateMap<DatingDto, UpdateDatingDto>();
    }
}