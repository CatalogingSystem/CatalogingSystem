using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;

namespace CatalogingSystem.DTOs.Mapping;

public class AdministrativeDataProfile : Profile
{
    public AdministrativeDataProfile()
    {
        CreateMap<AdministrativeDataDto, AdministrativeData>();
        CreateMap<AdministrativeData, AdministrativeDataDto>();
        CreateMap<UpdateAdministrativeDataDto, AdministrativeData>();
        CreateMap<CopiesReproductionsDto, CopiesReproductions>();
        CreateMap<CopiesReproductions, CopiesReproductionsDto>();
        CreateMap<ValuationDto, Valuation>();
        CreateMap<Valuation, ValuationDto>();
        CreateMap<CatalogerDto, Cataloger>();
        CreateMap<Cataloger, CatalogerDto>();
    }
}