using Schedule.App.Dto;
using Schedule.App.Models;

namespace Schedule.App.Helpers;

public static class RegimeHelper
{
    public static int GetCircleNumber(CalculationRegime regime, DateTime regimeStartDateUsing, DateTime date)
    {
        var difDays = (regimeStartDateUsing - date).Days;
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
        var firstDayInCircle = startDate.AddDays(regime.DaysCount * circleNumber);

        return (currDate - firstDayInCircle).Days;
    }
}

public class RegimeDay
{
    public int Number { get; set; }
    public bool IsWork { get; set; }
}