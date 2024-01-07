using System.Globalization;

namespace SalaryCalculation.Shared.Extensions.PeriodExtensions;

public static class PeriodExtensions
{
    public static int NextPeriod(this int period)
    {
        var month = period % 100;
        if (month < 12)
            return ++period;
        else
            return (period / 100 + 1) * 100 + 1;
    }
    
    public static int PreviousPeriod(this int period)
    {
        var month = period % 100;
        if (month > 1)
            return --period;
        else
            return (period / 100 - 1) * 100 + 12;
    }

    public static int AddPeriods(this int period, int months)
    {
        Func<int, int> function = months > 0 ? NextPeriod : PreviousPeriod;
        for (var m = 0; m < Math.Abs(months); m++)
            period = function(period);
        return period;
    }

    public static DateTime ToDateTime(this int period)
    {
        return new DateTime(period / 100, period % 100, 1);
    }

    public static string ToShortPeriodString(this int period)
    {
        return period.ToDateTime().ToString("MM.yyyy");
    }
    
    public static string ToLongPeriodString(this int period, CultureInfo culture)
    {
        return period.ToDateTime().ToString("MMMM yyyy", culture);
    }
    
    public static string ToLongUAPeriodString(this int period)
    {
        return period.ToDateTime().ToString("MMMM yyyy", new CultureInfo("uk-UA"));
    }

    public static string ToShortUADateString(this DateTime date)
    {
        return date.ToString("d", new CultureInfo("uk-UA"));
    }
    
    public static string ToLongUADateString(this DateTime date)
    {
        return date.ToString("dd MMMM yyyy", new CultureInfo("uk-UA"));
    }

    public static int ToPeriod(this DateTime date)
    {
        return date.Year * 100 + date.Month;
    }
}