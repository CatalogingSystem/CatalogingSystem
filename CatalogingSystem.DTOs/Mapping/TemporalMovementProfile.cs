using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;

namespace CatalogingSystem.DTOs.Mapping;

public class TemporalMovementProfile : Profile
{
    public TemporalMovementProfile()
    {
        CreateMap<TemporalMovementDto, TemporalMovement>();
        CreateMap<TemporalMovement, TemporalMovementDto>();
        CreateMap<UpdateTemporalMovementDto, TemporalMovement>();
        CreateMap<ApplicantDto, Applicant>();
        CreateMap<Applicant, ApplicantDto>();
        CreateMap<RepresentativeDto, Representative>();
        CreateMap<Representative, RepresentativeDto>();
        CreateMap<DepartureDto, Departure>();
        CreateMap<Departure, DepartureDto>();
        CreateMap<ReturnDto, Return>();
        CreateMap<Return, ReturnDto>();
    }
}
