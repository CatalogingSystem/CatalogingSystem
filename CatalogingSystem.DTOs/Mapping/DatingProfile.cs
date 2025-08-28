using AutoMapper;
using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;

namespace CatalogingSystem.DTOs.Mapping;

public class DatingProfile : Profile
{
    public DatingProfile()
    {
        CreateMap<DatingDto, Dating>().ForMember(dest => dest.Inventory, opt => opt.Ignore());
        CreateMap<Dating, DatingDto>();
        CreateMap<UpdateDatingDto, Dating>().ForMember(dest => dest.Inventory, opt => opt.Ignore());
        CreateMap<SimpleDateDto, SimpleDate>();
        CreateMap<SimpleDate, SimpleDateDto>();
        CreateMap<DateRangeDto, DateRange>();
        CreateMap<DateRange, DateRangeDto>();
        CreateMap<DateDetailDto, DateDetail>();
        CreateMap<DateDetail, DateDetailDto>();
        CreateMap<ApproximateDatingDto, ApproximateDating>();
        CreateMap<ApproximateDating, ApproximateDatingDto>();
        CreateMap<DatingNotesDto, DatingNotes>();
        CreateMap<DatingNotes, DatingNotesDto>();
    }
}
