using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;

namespace CatalogingSystem.DTOs.Mapping;

public class TenantCustomizationProfile : Profile
{
    public TenantCustomizationProfile()
    {
        CreateMap<TenantCustomization, TenantCustomizationDto>();
        CreateMap<TenantCustomizationDto, TenantCustomization>();
        CreateMap<UpdateTenantCustomizationDto, TenantCustomization>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}