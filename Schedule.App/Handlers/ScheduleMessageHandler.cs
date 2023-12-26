using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using Organization.Data.Data;
using Organization.Data.Entities;
using SalaryCalculation.Data.BaseHandlers;
using SalaryCalculation.Shared.Extensions.MoreLinq;
using Schedule.App.Commands;
using Schedule.App.Helpers;
using Schedule.App.Models;
using Schedule.Data.BaseModels;
using Schedule.Data.Data;
using Schedule.Data.Entities;
using Schedule.Data.Enums;

namespace Schedule.App.Handlers;

public class ScheduleMessageHandler : BaseMessageHandler<DaysSettingMessage>
{
    private IOrganizationUnitOfWork _organizationUnitOfWork;
    protected new IScheduleUnitOfWork Work;

    private readonly Time _morning = new(6, 0);
    private readonly Time _evening = new(18, 0);
    private readonly Time _night = new(22, 0);
    private readonly Time _day = new(24, 0);

    public ScheduleMessageHandler(IScheduleUnitOfWork work, IOrganizationUnitOfWork orgWork,
        ILogger<ScheduleMessageHandler> logger, IMapper mapper) : base(work, logger, mapper)
    {
        Work = work;
        _organizationUnitOfWork = orgWork;
    }
    
    public override async Task HandleAsync(DaysSettingMessage msg)
    {
        var regimes = await GetUsedRegimesAsync(msg);
        var workDays = await CreateDaysAsync(regimes, msg);
        var builder = Builders<EmpDay>.Filter;
        await DeleteExistingDatesAsync(workDays.Select(x => x.EmployeeId), msg);

        await Work.GetCollection<EmpDay>()
            .InsertManyAsync(workDays);
    }

    private async Task<List<EmpDay>> CreateDaysAsync(List<CalculationRegime> calculationRegimes,
        DaysSettingMessage msg)
    {
        var autoEmpDays = new List<EmpDay>();
        var holidays = (await LoadHolidaysAsync(msg.DateFrom.Year)).ToArray();
        var empDaysId = Work.GetCollection<EmpDay>().NewNumberId();

        await Parallel.ForEachAsync(calculationRegimes, async (regime, token) =>
        {
            var startDate = msg.DateFrom.Year == DateTime.Now.Year ? regime.StartDateInCurrentYear
                : msg.DateFrom.Year == DateTime.Now.Year - 1 ? regime.StartDateInPreviousYear
                : regime.StartDateInNextYear;
            if (!startDate.HasValue)
                return;
            var regimeDaysCircle = RegimeHelper.GetRegimeDaysCircle(regime);

            var employees = await _organizationUnitOfWork.GetCollection<Employee>()
                .Find(x => x.RegimeId == regime.RegimeId && (!x.DateTo.HasValue || x.DateTo.Value < msg.DateTo))
                .Project(x => new { x.Id, x.DateFrom, x.ShiftNumber })
                .ToListAsync();

            var reserveForHoliday = false;
            for (var (currDate, next) = (msg.DateFrom, msg.DateFrom.AddDays(1)); 
                 currDate <= msg.DateTo; (currDate, next) = (currDate.AddDays(1), next.AddDays(1)))
            {
                var nextDayHoliday = holidays.Contains(next);
                var hours = new Dictionary<int, HoursDetail>();
                
                for (var i = 0; i < regime.ShiftsCount; i++)
                {
                    var startDateForShift =
                        startDate.Value.AddDays(regime.WorkDayDetails.Sum(x => x.DaysOfWeek.Count()) * i);
                    var circleNumber = RegimeHelper.GetCircleNumber(regime, startDateForShift, currDate);
                    var dayOfCircle = RegimeHelper.GetDayOfCircle(regime, startDateForShift, circleNumber, currDate);
                    if (dayOfCircle > regime.DaysCount)
                    {
                        circleNumber++;
                        dayOfCircle = 1;
                    }
                    
                    if (!regimeDaysCircle[dayOfCircle])
                        continue;
                    
                    var isHoliday = holidays.Contains(currDate);
                    var workDayDetail = regime.WorkDayDetails
                        .First(x => x.DaysOfWeek.Any(d => d.DayOfCircle == dayOfCircle));
                    if (isHoliday && !workDayDetail.IsHolidayWork)
                        continue;
                    if (reserveForHoliday)
                    {
                        reserveForHoliday = false;
                        continue;
                    }
                    if (!regimeDaysCircle[dayOfCircle != regime.DaysCount ? dayOfCircle + 1 : 1] && nextDayHoliday
                        && !workDayDetail.IsHolidayWork)
                        reserveForHoliday = true;
                    var newDetail = workDayDetail;
                    
                    var hoursForShifts = msg.Type == EDayType.Work
                        ? CreateHoursFromRegime(newDetail, isHoliday, nextDayHoliday)
                        : new HoursDetail();
                    hours.Add(i+1, hoursForShifts);
                }
                
                employees.ParallelForEach(employee =>
                {
                    if (currDate < employee.DateFrom || !hours.TryGetValue(employee.ShiftNumber, out var hoursDetail))
                        return;
                    autoEmpDays.Add(new EmpDay()
                    {
                        Id = empDaysId++,
                        Date = currDate,
                        DayType = (int)msg.Type,
                        Hours = hoursDetail,
                        EmployeeId = employee.Id,
                        OrganizationId = msg.OrganizationId
                    });
                });
            }
        });

        return autoEmpDays;
    }

    private HoursDetail CreateHoursFromRegime(WorkDayDetail workDayDetail, bool isHoliday, bool nextDayHoliday)
    {
        var startTime = isHoliday ? workDayDetail.StartTimeInHoliday : workDayDetail.StartTime;
        var endTime = isHoliday
            ? workDayDetail.IsEndTimeInHolidayNextDay ?? false
                ? workDayDetail.EndTimeInHoliday + _day
                : workDayDetail.EndTimeInHoliday
            : workDayDetail.IsEndTimeNextDay
                ? workDayDetail.EndTime + _day
                : workDayDetail.EndTime;

        var shortDay = nextDayHoliday && (workDayDetail.IsHolidayShort ?? false) ? decimal.One : decimal.Zero;
        
        var summaryHours = Time.ConvertToDecimal(endTime - startTime) - 
                           (!workDayDetail.IsLaunchPaid ? workDayDetail.LaunchTime : decimal.Zero) - shortDay;
        var dayHours = GetWorkTimeByType(ref startTime, endTime, _morning, _evening) -
            (!workDayDetail.IsLaunchPaid ? workDayDetail.LaunchTime : decimal.Zero) - shortDay; // 06:00 - 18:00
        var eveningHours = GetWorkTimeByType(ref startTime, endTime, _evening, _night); // 18:00 - 22:00
        var nightHours = GetWorkTimeByType(ref startTime, endTime, _night, _morning+_day); // 22:00 - 06:00
        var extraDay = GetWorkTimeByType(ref startTime, endTime, _morning + _day, _evening + _day);
        var extraEvening = GetWorkTimeByType(ref startTime, endTime, _evening + _day, _night + _day);
        var extraNight = GetWorkTimeByType(ref startTime, endTime, _night + _day, _morning + _day + _day);
        return new HoursDetail()
        {
            Day = dayHours + extraDay,
            Evening = eveningHours + extraEvening,
            Night = nightHours + extraNight,
            Holiday = isHoliday,
            Summary = summaryHours,
        };
    }

    private decimal GetWorkTimeByType(ref Time workStart, Time workEnd, Time start, Time end)
    {
        var total = 0m;
        if (workStart >= start && workStart < end)
        {
            if (workEnd <= end)
                total = Time.ConvertToDecimal(workEnd - workStart);
            else
            {
                total = Time.ConvertToDecimal(end - workStart);
                workStart = end;
            }
        }
        
        return total;
    }

    private async Task<List<CalculationRegime>> GetUsedRegimesAsync(DaysSettingMessage msg)
    {
        var searchFilter = await GetRegimeSearchFilterAsync(msg);
        var regimes = await Work.GetCollection<Regime>()
            .Find(searchFilter)
            .Project(x => new CalculationRegime()
            {
                RegimeId = x.Id,
                WorkDayDetails = x.WorkDayDetails,
                RestDays = x.RestDayDetails,
                IsCircle = x.IsCircle,
                StartDateInNextYear = x.StartDateInNextYear,
                StartDateInPreviousYear = x.StartDateInPreviousYear,
                StartDateInCurrentYear = x.StartDateInCurrentYear,
                ShiftsCount = x.ShiftsCount
            }).ToListAsync();
        return regimes;
    }


    private async Task<FilterDefinition<Regime>> GetRegimeSearchFilterAsync(DaysSettingMessage msg)
    {
        var builder = Builders<Regime>.Filter;
        var definition = new List<FilterDefinition<Regime>>
        {
            builder.Eq(x => x.OrganizationId, msg.OrganizationId)
        };
        if (msg.OrganizationUnitId.HasValue || msg.PositionId.HasValue)
        {
            var regimeIds = await _organizationUnitOfWork.GetCollection<Employee>()
                .Find(x => x.Organization.Id == msg.OrganizationId
                           && (!msg.OrganizationUnitId.HasValue ||
                               x.OrganizationUnit.Id == msg.OrganizationUnitId.Value)
                           && (!msg.PositionId.HasValue || x.Position.Id == msg.PositionId.Value))
                .Project(x => x.RegimeId)
                .ToListAsync();
            definition.Add(builder.In(x => x.Id, regimeIds.Distinct()));
        }
        if(msg.RegimeId.HasValue)
            definition.Add(builder.Eq(x => x.Id, msg.RegimeId.Value));

        return builder.And(definition);
    }

    private async Task<IEnumerable<DateTime>> LoadHolidaysAsync(int year, string region = "UA")
    {
        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync($"https://date.nager.at/api/v3/PublicHolidays/{year}/{region}");
            if (response.IsSuccessStatusCode)
            {
                var holidaysJson = await response.Content.ReadAsStringAsync();
                return (JsonConvert.DeserializeObject<Holiday[]>(holidaysJson) ?? Array.Empty<Holiday>()).Select(x =>
                    x.Date);
            }
            else
            {
                throw new HttpRequestException();
            }
        }
    }

    private Task<DeleteResult> DeleteExistingDatesAsync(IEnumerable<int> empIds, DaysSettingMessage msg)
    {
        var builder = Builders<EmpDay>.Filter;
        var result = Work.GetCollection<EmpDay>()
            .DeleteManyAsync(builder.In(x => x.EmployeeId, empIds)
                             & builder.Gte(x => x.Date, msg.DateFrom)
                             & builder.Lte(x => x.Date, msg.DateTo)
                             & builder.Eq(x => x.OrganizationId, msg.OrganizationId));

        return result;
    }

    private class Holiday
    {
        public DateTime Date { get; set; }
        public string LocalName { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public bool Fixed { get; set; }
        public bool Global { get; set; }
        public object[] Counties { get; set; }
        public object LaunchYear { get; set; }
        public string[] Types { get; set; }
    }
}