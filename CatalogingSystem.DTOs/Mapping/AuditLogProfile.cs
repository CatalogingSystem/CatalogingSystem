namespace CatalogingSystem.DTOs.Mapping;

using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs;

public class AuditLogProfile : Profile
{
    public AuditLogProfile()
    {
        CreateMap<AuditLog, AuditLogDto>()
            .ForMember(dest => dest.ActionTimestamp, opt => opt.MapFrom(src => src.ActionTimestamp.AddHours(-4)));
    }
}