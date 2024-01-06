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
        CreateMap<Regime, RegimeDto>()
            .ForMember(dst => dst.RestDays, scr => scr.MapFrom(x => string.Join(", ", x.RestDayDetails)))
            .ForMember(dst => dst.WorkDays, scr => scr.MapFrom(x => x.WorkDayDetails));

        CreateMap<Regime, RegimeUpdateCommand>().ReverseMap();
        
        CreateMap<Regime, RegimeCreateCommand>().ReverseMap();

        CreateMap<WorkDayDetail, WorkDayDetailDto>()
            .ForMember(dst => dst.DaysOfWeek, scr => scr.MapFrom(x => string.Join(", ", x.DaysOfWeek)));

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