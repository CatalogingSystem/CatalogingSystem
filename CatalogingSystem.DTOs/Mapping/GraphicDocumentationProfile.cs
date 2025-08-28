namespace CatalogingSystem.DTOs.Mapping;

using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;

public class GraphicDocumentationProfile : Profile
{
    public GraphicDocumentationProfile()
    {
        CreateMap<GraphicDocumentationDto, GraphicDocumentation>()
            .ForMember(dest => dest.inventory, opt => opt.Ignore());
        CreateMap<GraphicDocumentation, GraphicDocumentationDto>();
        CreateMap<UpdateGraphicDocumentationDto, GraphicDocumentation>()
            .ForMember(dest => dest.inventory, opt => opt.Ignore());
        CreateMap<Dimensions, Dimensions>();
        CreateMap<ImageAuthor, ImageAuthor>();
    }
}
