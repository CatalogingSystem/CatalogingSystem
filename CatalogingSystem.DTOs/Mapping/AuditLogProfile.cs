using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;

namespace CatalogingSystem.DTOs.Mapping;

public class AuditLogProfile : Profile
{
    public AuditLogProfile()
    {
        CreateMap<AuditLog, AuditLogDto>();
    }
}