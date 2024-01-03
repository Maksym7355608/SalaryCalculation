using Newtonsoft.Json;
using SalaryCalculation.Shared.Extensions.PeriodExtensions;
using Schedule.App.Dto;
using Schedule.App.Models;
using Schedule.Data.BaseModels;

namespace Schedule.App.Helpers;

public static class RegimeHelper
{
    private static readonly Time Morning = new(6, 0);
    private static readonly Time Evening = new(18, 0);
    private static readonly Time Night = new(22, 0);
    private static readonly Time Day = new(24, 0);
    
    public static async Task<Dictionary<int, HoursDetails>> GetHoursDetailAsync(CalculationRegime regime, int period)
    {
        var year = period / 100;
        var holidays = await LoadHolidaysAsync(year);
        var startDate = year == DateTime.Now.Year ? regime.StartDateInCurrentYear
            : year == DateTime.Now.Year - 1 ? regime.StartDateInPreviousYear
            : regime.StartDateInNextYear;
        if (!startDate.HasValue)
            return null;

        var regimeDaysCircle = GetRegimeDaysCircle(regime);

        var reserveForHoliday = false;
        var firstDay = period.ToDateTime();
        var hours = new List<List<HoursDetail>>(regime.ShiftsCount);
        for (var (currDate, next) = (firstDay, firstDay.AddDays(1));
             currDate < firstDay.AddMonths(1);
             (currDate, next) = (currDate.AddDays(1), next.AddDays(1)))
        {
            var nextDayHoliday = holidays.Contains(next);

            for (var i = 0; i < regime.ShiftsCount; i++)
            {
                var startDateForShift =
                    startDate.Value.AddDays(regime.WorkDayDetails.Sum(x => x.DaysOfWeek.Count()) * i);
                var circleNumber = GetCircleNumber(regime, startDateForShift, currDate);
                var dayOfCircle = GetDayOfCircle(regime, startDateForShift, circleNumber, currDate);
                if (dayOfCircle > regime.DaysCount)
                    dayOfCircle = 1;

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

                var hoursForShifts = CreateHoursFromRegime(newDetail, isHoliday, nextDayHoliday);
                if(hours.Count > i)
                    hours[i].Add(hoursForShifts);
                else
                    hours.Add(new List<HoursDetail>(){hoursForShifts});
            }
        }

        return hours.Select(h =>
        {
            var empDays = h;
            var holidayHours = h.Where(x => x.Holiday).ToArray();
            return (hours.IndexOf(h)+1, new HoursDetails()
            {
                Summary = empDays.Sum(x => x.Summary),
                Day = empDays.Sum(x => x.Day),
                Evening = empDays.Sum(x => x.Evening),
                Night = empDays.Sum(x => x.Night),
                HolidaySummary = holidayHours.Sum(x => x.Summary),
                HolidayDay = holidayHours.Sum(x => x.Day),
                HolidayNight = holidayHours.Sum(x => x.Night),
                HolidayEvening = holidayHours.Sum(x => x.Evening),
            });
        }).ToDictionary(k => k.Item1, v => v.Item2);
    }

    public static int GetCircleNumber(CalculationRegime regime, DateTime regimeStartDateUsing, DateTime date)
    {
        var difDays = (date - regimeStartDateUsing).Days;
        return difDays / regime.DaysCount;
    }

    public static (IEnumerable<DayDto> work, IEnumerable<DayDto> rest) GetCircleInfo(CalculationRegime regime, DateTime startDate, int circleNumber)
    {
        var work = new List<DayDto>();
        var rest = new List<DayDto>();
        var regimeDaysCircle = GetRegimeDaysCircle(regime);
        var currWeek = 1;
        for (var currDayNumber = 1; currDayNumber <= regime.DaysCount; currDayNumber++)
        {
            if(regimeDaysCircle[currDayNumber])
                work.Add(new DayDto()
                {
                    DayOfCircle = currDayNumber,
                    WeekDay = startDate.AddDays(currDayNumber - 1).DayOfWeek,
                    Week = startDate.AddDays(currDayNumber - 1).DayOfWeek == DayOfWeek.Sunday ? currWeek++ : currWeek,
                });
            else
                rest.Add(new DayDto()
                {
                    DayOfCircle = currDayNumber,
                    WeekDay = startDate.AddDays(currDayNumber - 1).DayOfWeek,
                    Week = startDate.AddDays(currDayNumber - 1).DayOfWeek == DayOfWeek.Sunday ? currWeek++ : currWeek,
                });
        }

        return (work, rest);
    }

    public static Dictionary<int, bool> GetRegimeDaysCircle(CalculationRegime regime)
    {
        return regime.WorkDayDetails.SelectMany(x => x.DaysOfWeek)
            .Select(workDay => new RegimeDay()
            {
                IsWork = true,
                Number = workDay.DayOfCircle
            }).Concat(regime.RestDays.Select(workDay => new RegimeDay()
            {
                IsWork = false,
                Number = workDay.DayOfCircle
            })).OrderBy(x => x.Number).ToDictionary(k => k.Number, v => v.IsWork);
    }

    public static int GetDayOfCircle(CalculationRegime regime, DateTime startDate, int circleNumber, DateTime currDate)
    {
        var firstDayInCircle = startDate.AddDays(regime.DaysCount * circleNumber - 1);

        return (currDate - firstDayInCircle).Days;
    }
    
    public static HoursDetail CreateHoursFromRegime(WorkDayDetail workDayDetail, bool isHoliday, bool nextDayHoliday)
    {
        var startTime = isHoliday ? workDayDetail.StartTimeInHoliday : workDayDetail.StartTime;
        var endTime = isHoliday
            ? workDayDetail.IsEndTimeInHolidayNextDay ?? false
                ? workDayDetail.EndTimeInHoliday + Day
                : workDayDetail.EndTimeInHoliday
            : workDayDetail.IsEndTimeNextDay
                ? workDayDetail.EndTime + Day
                : workDayDetail.EndTime;

        var shortDay = nextDayHoliday && (workDayDetail.IsHolidayShort ?? false) ? decimal.One : decimal.Zero;
        
        var summaryHours = Time.ConvertToDecimal(endTime - startTime) - 
                           (!workDayDetail.IsLaunchPaid ? workDayDetail.LaunchTime : decimal.Zero) - shortDay;
        var dayHours = GetWorkTimeByType(ref startTime, endTime, Morning, Evening) -
            (!workDayDetail.IsLaunchPaid ? workDayDetail.LaunchTime : decimal.Zero) - shortDay; // 06:00 - 18:00
        var eveningHours = GetWorkTimeByType(ref startTime, endTime, Evening, Night); // 18:00 - 22:00
        var nightHours = GetWorkTimeByType(ref startTime, endTime, Night, Morning+Day); // 22:00 - 06:00
        var extraDay = GetWorkTimeByType(ref startTime, endTime, Morning + Day, Evening + Day);
        var extraEvening = GetWorkTimeByType(ref startTime, endTime, Evening + Day, Night + Day);
        var extraNight = GetWorkTimeByType(ref startTime, endTime, Night + Day, Morning + Day + Day);
        return new HoursDetail()
        {
            Day = dayHours + extraDay,
            Evening = eveningHours + extraEvening,
            Night = nightHours + extraNight,
            Holiday = isHoliday,
            Summary = summaryHours,
        };
    }

    private static decimal GetWorkTimeByType(ref Time workStart, Time workEnd, Time start, Time end)
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
    
    public static async Task<DateTime[]> LoadHolidaysAsync(int year, string region = "UA")
    {
        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync($"https://date.nager.at/api/v3/PublicHolidays/{year}/{region}");
            if (response.IsSuccessStatusCode)
            {
                var holidaysJson = await response.Content.ReadAsStringAsync();
                return (JsonConvert.DeserializeObject<Holiday[]>(holidaysJson) ?? Array.Empty<Holiday>()).Select(x =>
                    x.Date).ToArray();
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

public class RegimeDay
{
    public int Number { get; set; }
    public bool IsWork { get; set; }
}