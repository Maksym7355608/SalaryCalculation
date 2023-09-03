using System.Globalization;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using Organization.Data.Data;
using Organization.Data.Entities;
using Progress.App;
using SalaryCalculation.Shared.Extensions.MoreLinq;
using SalaryCalculation.Shared.Extensions.PeriodExtensions;
using Schedule.App.Abstract;
using Schedule.App.Commands;
using Schedule.App.Dto;
using Schedule.App.Helpers;
using Schedule.Data.BaseModels;
using Schedule.Data.Data;
using Schedule.Data.Entities;
using Schedule.Data.Enums;

namespace Schedule.App.Handlers;

public class ScheduleReaderLogic : BaseScheduleCommandHandler, IScheduleReaderLogic
{
    private IOrganizationUnitOfWork _organizationUnitOfWork;

    public ScheduleReaderLogic(IScheduleUnitOfWork work, IOrganizationUnitOfWork orgWork,
        ILogger<ScheduleReaderLogic> logger, IMapper mapper) : base(work, logger, mapper)
    {
        _organizationUnitOfWork = orgWork;
    }

    public async Task<bool> CalculatePeriodCalendarAsync(int employeeId, int period, int regimeId)
    {
        var dateFrom = period.ToDateTime();
        var dateTo = dateFrom.AddMonths(1).AddDays(-1);
        var filter = GetPeriodCalendarFilter(dateFrom, dateTo, employeeId);

        var empDays = await Work.GetCollection<EmpDay>()
            .Find(filter)
            .ToListAsync();

        var calendar = CreateCalendar(empDays, period, regimeId);

        var existCalendar = await Work.GetCollection<PeriodCalendar>()
            .UpdateOneAsync(x => x.EmployeeId == employeeId && x.Period == period,
                Builders<PeriodCalendar>.Update.Set(x => x, calendar));
        if (existCalendar.ModifiedCount == 0)
        {
            await Work.GetCollection<PeriodCalendar>().InsertOneAsync(calendar);
            return true;
        }

        return existCalendar.ModifiedCount > 0;
    }

    public async Task<string> MassCalculatePeriodCalendarAsync(PeriodCalendarMassCalculateCommand command)
    {
        var progressId = Guid.NewGuid().ToString();
        var progressMsg = new ProgressCreateMessage()
        {
            ProgressId = progressId,
        };
        await Work.MessageBroker.PublishAsync(progressMsg);

        var msg = new PeriodCalendarMassCalculateMessage()
        {
            PeriodFrom = command.PeriodFrom,
            PeriodTo = command.PeriodTo,
            OrganizationId = command.OrganizationId,
            RegimeIds = command.RegimeIds
        };
        await Work.MessageBroker.PublishAsync(msg);
        return progressId;
    }

    private PeriodCalendar CreateCalendar(List<EmpDay> empDays, int period, int regimeId)
    {
        return new PeriodCalendar()
        {
            EmployeeId = empDays.First().EmployeeId,
            Hours = new HoursDetail()
            {
                Summary = empDays.Sum(x => x.Hours.Summary),
                Day = empDays.Sum(x => x.Hours.Day),
                Evening = empDays.Sum(x => x.Hours.Evening),
                Night = empDays.Sum(x => x.Hours.Night),
                Holiday = empDays.Sum(x => x.Hours.Holiday)
            },
            OrganizationId = empDays.First().OrganizationId,
            Period = period,
            SickLeave = empDays.Where(x => x.DayType == (int)EDayType.Sick).Count(),
            VacationDays = empDays.Where(x => x.DayType == (int)EDayType.Holiday).Count(),
            WorkDays = empDays.Where(x => x.DayType == (int)EDayType.Work).Count(),
            RegimeId = regimeId,
        };
    }

    private FilterDefinition<EmpDay> GetPeriodCalendarFilter(DateTime dateFrom, DateTime dateTo, int employeeId)
    {
        var filter = Builders<EmpDay>.Filter;
        return filter.Eq(x => x.EmployeeId, employeeId) &
               filter.Gte(x => x.Date, dateFrom) &
               filter.Lte(x => x.Date, dateTo);
    }


    public async Task<bool> QuickSettingDays(DaysSettingFilter filter)
    {
        var regimes = await GetUsedRegimesAsync(filter);
        var workDays = await CreateDaysAsync(regimes, filter);
        //TODO: заінсертити нові дні які вже є, оновити вже записані дні
        return false;
    }

    private async Task<List<EmpDayDto>> CreateDaysAsync(List<CalculationRegimeDto> calculationRegimes,
        DaysSettingFilter filter)
    {
        var autoEmpDays = new List<EmpDayDto>();
        var holidays = (await LoadHolidaysAsync(filter.DateFrom.Year)).ToArray();

        await Parallel.ForEachAsync(calculationRegimes, async (regime, token) =>
        {
            var useStartDate = filter.DateFrom.Year == DateTime.Now.Year ? regime.StartDateInCurrentYear
                : filter.DateFrom.Year == DateTime.Now.Year - 1 ? regime.StartDateInPreviousYear
                : regime.StartDateInNextYear;
            if (!useStartDate.HasValue)
                return;
            var circleNumber = RegimeHelper.GetCircleNumber(regime, useStartDate.Value, filter.DateFrom);
            var regimeDaysCircle = RegimeHelper.GetRegimeDaysCircle(regime);

            var employees = await _organizationUnitOfWork.GetCollection<Employee>()
                .Find(x => x.RegimeId == regime.RegimeId)
                .Project(x => new { x.Id, x.DateFrom })
                .ToListAsync();

            for (var currDate = filter.DateFrom; currDate < filter.DateTo; currDate = currDate.AddDays(1))
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
                            autoEmpDays.Add(new EmpDayDto()
                            {
                                Date = currDate,
                                DayType = filter.Type,
                                Hours = filter.Type == EDayType.Work
                                    ? CreateHoursFromRegime(regime, dayOfCircle, isHoliday)
                                    : new HoursDetailDto(),
                                EmployeeId = employee.Id,
                                OrganizationId = filter.OrganizationId
                            });
                    });
                }
            }
        });

        return autoEmpDays;
    }

    private HoursDetailDto CreateHoursFromRegime(CalculationRegimeDto regime, int dayOfCircle, bool isHoliday)
    {
        var workDayDetails = regime.WorkDays.First(x => x.DaysOfWeek.Any(d => d.DayOfCircle == dayOfCircle));
        var summaryHours = TimeDto.ConvertToDecimal(
            (workDayDetails.IsEndTimeNextDay ? workDayDetails.EndTime + new TimeDto(24, 0) : workDayDetails.EndTime) -
            workDayDetails.StartTime);
        var dayHours = GetWorkTimeByType(workDayDetails, new TimeDto(6, 0), new TimeDto(18, 0)); // 06:00 - 18:00
        var eveningHours = GetWorkTimeByType(workDayDetails, new TimeDto(18, 0), new TimeDto(22, 0)); // 18:00 - 22:00
        var nightHours = GetWorkTimeByType(workDayDetails, new TimeDto(22, 0), new TimeDto(6, 0)); // 22:00 - 06:00
        var holidayHours = workDayDetails.IsHolidayWork && isHoliday
            ? TimeDto.ConvertToDecimal(workDayDetails.EndTimeInHoliday - workDayDetails.StartTimeInHoliday)
            : 0; // when holiday if regime.IsHolidayWork is true
        return new HoursDetailDto()
        {
            Day = dayHours,
            Evening = eveningHours,
            Night = nightHours,
            Holiday = holidayHours,
            Summary = summaryHours,
        };
    }

    private decimal GetWorkTimeByType(WorkDayDetailDto workDay, TimeDto start, TimeDto end)
    {
        var total = 0m;
        if (workDay.StartTime >= start && workDay.StartTime <= end)
        {
            if (workDay.EndTime <= end)
                total = TimeDto.ConvertToDecimal(
                    (workDay.IsEndTimeNextDay ? workDay.EndTime + new TimeDto(24, 0) : workDay.EndTime) -
                    workDay.StartTime);
            else
                total = TimeDto.ConvertToDecimal(
                    (start > end ? end + new TimeDto(24, 0) : end) -
                    workDay.StartTime);
        }

        return total;
    }

    private async Task<List<CalculationRegimeDto>> GetUsedRegimesAsync(DaysSettingFilter filter)
    {
        var searchFilter = await GetRegimeSearchFilterAsync(filter);
        var regimes = Work.GetCollection<Regime>()
            .Find(searchFilter)
            .Project(x => new CalculationRegimeDto()
            {
                RegimeId = x.Id,
                DaysCount = x.DaysCount,
                WorkDays = Mapper.Map<IEnumerable<WorkDayDetailDto>>(x.WorkDayDetails),
                RestDays = Mapper.Map<IEnumerable<DayDto>>(x.RestDayDetails),
                IsCircle = x.IsCircle,
                StartDateInNextYear = x.StartDateInNextYear,
                StartDateInPreviousYear = x.StartDateInPreviousYear,
                StartDateInCurrentYear = x.StartDateInCurrentYear,
            }).ToListAsync();
        return await regimes;
    }


    private async Task<FilterDefinition<Regime>> GetRegimeSearchFilterAsync(DaysSettingFilter filter)
    {
        var builder = Builders<Regime>.Filter;
        var definition = new List<FilterDefinition<Regime>>
        {
            builder.Eq(x => x.OrganizationId, filter.OrganizationId)
        };
        if (filter.OrganizationUnitId.HasValue || filter.PositionId.HasValue)
        {
            var regimeIds = await _organizationUnitOfWork.GetCollection<Employee>()
                .Find(x => x.Organization.Id == filter.OrganizationId
                           && (!filter.OrganizationUnitId.HasValue ||
                               x.OrganizationUnit.Id == filter.OrganizationUnitId.Value)
                           && (!filter.PositionId.HasValue || x.Position.Id == filter.PositionId.Value))
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