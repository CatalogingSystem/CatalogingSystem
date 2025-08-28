namespace CatalogingSystem.DTOs.Mapping;

using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;

public class ArchivoAdministrativoProfile : Profile
{
    public ArchivoAdministrativoProfile()
    {
        CreateMap<ArchivoAdministrativoDto, ArchivoAdministrativo>();
        CreateMap<ArchivoAdministrativo, ArchivoAdministrativoDto>();
        CreateMap<ArchivoAdministrativo, ArchivoAdministrativo>();
    }
}
