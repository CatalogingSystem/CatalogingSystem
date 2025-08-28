using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;

namespace CatalogingSystem.DTOs.Mapping;

public class ConservationProfile : Profile
{
    public ConservationProfile()
    {
        CreateMap<ConservationDto, Conservation>()
            .ForMember(dest => dest.Inventory, opt => opt.Ignore());
        CreateMap<Conservation, ConservationDto>();
        CreateMap<UpdateConservationDto, Conservation>()
            .ForMember(dest => dest.Inventory, opt => opt.Ignore());
    }
}
