namespace CatalogingSystem.DTOs.Mapping;

using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;

public class IdentificationProfile : Profile
{
    public IdentificationProfile()
    {
        CreateMap<IdentificationDto, Identification>();
        CreateMap<UpdateIdentificationDto, Identification>();
        CreateMap<Identification, IdentificationDto>();
        CreateMap<SectionDto, Section>();
        CreateMap<Section, SectionDto>();
        CreateMap<TypologyDto, Typology>();
        CreateMap<Typology, TypologyDto>();
        CreateMap<SpecificNameDto, SpecificName>();
        CreateMap<SpecificName, SpecificNameDto>();
        CreateMap<AuthorDto, Author>();
        CreateMap<Author, AuthorDto>();
        CreateMap<TitleDto, Title>();
        CreateMap<Title, TitleDto>();
        CreateMap<MaterialDto, Material>();
        CreateMap<Material, MaterialDto>();
        CreateMap<TechniquesDto, Technique>();
        CreateMap<Technique, TechniquesDto>();
    }
}