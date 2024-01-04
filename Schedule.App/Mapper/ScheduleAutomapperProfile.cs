using AutoMapper;
using Schedule.App.Commands;
using Schedule.App.Dto;
using Schedule.Data.BaseModels;
using Schedule.Data.Entities;
using Schedule.Data.Enums;

namespace Schedule.App.Mapper;

public class ScheduleAutomapperProfile : Profile
{
    public ScheduleAutomapperProfile()
    {
        CreateMap<Regime, RegimeDto>().ReverseMap();

        CreateMap<Regime, RegimeUpdateCommand>().ReverseMap();
        
        CreateMap<Regime, RegimeCreateCommand>().ReverseMap();

        CreateMap<WorkDayDetail, WorkDayDetailDto>().ReverseMap();

        CreateMap<PeriodCalendar, PeriodCalendarDto>()
            .ForMember(dst => dst.Regime, src => src.Ignore())
            .ReverseMap();

        CreateMap<HoursDetail, HoursDetailDto>().ReverseMap();

        CreateMap<HoursDetails, HoursDetailsDto>().ReverseMap();

        CreateMap<EmpDay, EmpDayDto>().ReverseMap();

        CreateMap<EmpDay, WorkDayCreateCommand>()
            .ForMember(x => x.DayType, y => y.MapFrom(z => (EDayType)z.DayType))
            .ReverseMap();

        CreateMap<Time, TimeDto>().ReverseMap();
    }
}