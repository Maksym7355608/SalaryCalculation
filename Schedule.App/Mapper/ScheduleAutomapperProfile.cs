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

        CreateMap<Regime, CalculationRegimeDto>().ReverseMap();

        CreateMap<Regime, RegimeUpdateCommand>().ReverseMap();
        
        CreateMap<Regime, RegimeCreateCommand>().ReverseMap();

        CreateMap<WorkDayDetail, WorkDayDetailDto>().ReverseMap();

        CreateMap<PeriodCalendar, PeriodCalendarDto>().ReverseMap();

        CreateMap<HoursDetail, HoursDetailDto>().ReverseMap();

        CreateMap<EmpDay, EmpDayDto>().ReverseMap();

        CreateMap<EmpDay, WorkDayCreateCommand>()
            .ForMember(x => x.DayType, y => y.MapFrom(z => (EDayType)z.DayType))
            .ReverseMap();

        CreateMap<Day, DayDto>()
            .ForMember(x => x.WeekDay, y => y.MapFrom(z => (DayOfWeek)z.WeekDay))
            .ReverseMap();

        CreateMap<Time, TimeDto>().ReverseMap();
    }
}