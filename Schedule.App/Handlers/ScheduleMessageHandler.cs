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

    public ScheduleMessageHandler(IScheduleUnitOfWork work, IOrganizationUnitOfWork orgWork,
        ILogger<ScheduleMessageHandler> logger, IMapper mapper) : base(work, logger, mapper)
    {
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

        await Parallel.ForEachAsync(calculationRegimes, async (regime, token) =>
        {
            var useStartDate = msg.DateFrom.Year == DateTime.Now.Year ? regime.StartDateInCurrentYear
                : msg.DateFrom.Year == DateTime.Now.Year - 1 ? regime.StartDateInPreviousYear
                : regime.StartDateInNextYear;
            if (!useStartDate.HasValue)
                return;
            var circleNumber = RegimeHelper.GetCircleNumber(regime, useStartDate.Value, msg.DateFrom);
            var regimeDaysCircle = RegimeHelper.GetRegimeDaysCircle(regime);

            var employees = await _organizationUnitOfWork.GetCollection<Employee>()
                .Find(x => x.RegimeId == regime.RegimeId)
                .Project(x => new { x.Id, x.DateFrom })
                .ToListAsync();

            for (var currDate = msg.DateFrom; currDate < msg.DateTo; currDate = currDate.AddDays(1))
            {
                var dayOfCircle = RegimeHelper.GetDayOfCircle(regime, useStartDate.Value, circleNumber, currDate);
                if (dayOfCircle > regime.DaysCount)
                {
                    circleNumber++;
                    dayOfCircle = 1;
                }

                if (regimeDaysCircle[dayOfCircle])
                {
                    var isHoliday = holidays.Contains(currDate);
                    employees.ParallelForEach(employee =>
                    {
                        if (currDate >= employee.DateFrom)
                            autoEmpDays.Add(new EmpDay()
                            {
                                Date = currDate,
                                DayType = (int)msg.Type,
                                Hours = msg.Type == EDayType.Work
                                    ? CreateHoursFromRegime(regime, dayOfCircle, isHoliday)
                                    : new HoursDetail(),
                                EmployeeId = employee.Id,
                                OrganizationId = msg.OrganizationId
                            });
                    });
                }
            }
        });

        return autoEmpDays;
    }

    private HoursDetail CreateHoursFromRegime(CalculationRegime regime, int dayOfCircle, bool isHoliday)
    {
        var workDayDetails = regime.WorkDayDetails.First(x => x.DaysOfWeek.Any(d => d.DayOfCircle == dayOfCircle));
        var startTime = isHoliday ? workDayDetails.StartTimeInHoliday : workDayDetails.StartTime;
        var endTime = isHoliday
            ? workDayDetails.IsEndTimeInHolidayNextDay
                ? workDayDetails.EndTimeInHoliday + new Time(24, 0)
                : workDayDetails.EndTimeInHoliday
            : workDayDetails.IsEndTimeNextDay
                ? workDayDetails.EndTime + new Time(24, 0)
                : workDayDetails.EndTime;
        
        var summaryHours = Time.ConvertToDecimal(endTime - startTime);
        var dayHours = GetWorkTimeByType(ref startTime, endTime, new Time(6, 0), new Time(18, 0)); // 06:00 - 18:00
        var eveningHours = GetWorkTimeByType(ref startTime, endTime, new Time(18, 0), new Time(22, 0)); // 18:00 - 22:00
        var nightHours = GetWorkTimeByType(ref startTime, endTime, new Time(22, 0), new Time(6, 0)); // 22:00 - 06:00
        return new HoursDetail()
        {
            Day = dayHours,
            Evening = eveningHours,
            Night = nightHours,
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
        var regimes = Work.GetCollection<Regime>()
            .Find(searchFilter)
            .Project(x => new CalculationRegime()
            {
                RegimeId = x.Id,
                DaysCount = x.DaysCount,
                WorkDayDetails = x.WorkDayDetails,
                RestDays = x.RestDayDetails,
                IsCircle = x.IsCircle,
                StartDateInNextYear = x.StartDateInNextYear,
                StartDateInPreviousYear = x.StartDateInPreviousYear,
                StartDateInCurrentYear = x.StartDateInCurrentYear,
            }).ToListAsync();
        return await regimes;
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

        return builder.And(definition);
    }

    private async Task<IEnumerable<DateTime>> LoadHolidaysAsync(int year, string region = "UA")
    {
        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync($"https://date.nager.at/Api/v2/PublicHoliday/{year}/{region}");
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