using Schedule.Data.Enums;

namespace Schedule.App.Dto;

public class DayDto
{
    public DayOfWeek WeekDay { get; set; }

    public int Week { get; set; }
    public int DayOfCircle { get; set; }
}

public class TimeDto
{
    public string TimeValue => $"{Hour}:{Minutes}";
    public int Hour { get; set; }
    public int Minutes { get; set; }

    public TimeDto(int hour, int minutes)
    {
        Hour = hour;
        Minutes = minutes;
    }
    
    public static bool operator >(TimeDto a, TimeDto b)
    {
        return CompareTo(a, b) > 0;
    }

    public static bool operator <(TimeDto a, TimeDto b)
    {
        return CompareTo(a, b) < 0;
    }
    
    public static bool operator >=(TimeDto a, TimeDto b)
    {
        return CompareTo(a, b) >= 0;
    }

    public static bool operator <=(TimeDto a, TimeDto b)
    {
        return CompareTo(a, b) <= 0;
    }
    
    public static bool operator !=(TimeDto a, TimeDto b)
    {
        return CompareTo(a, b) != 0;
    }
    
    public static bool operator ==(TimeDto a, TimeDto b)
    {
        return CompareTo(a, b) == 0;
    }

    public static TimeDto operator +(TimeDto a, TimeDto b)
    {
        var sumH = a.Hour + b.Hour + (a.Minutes + b.Minutes) / 60;
        var sumM = (a.Minutes + b.Minutes) % 60;
        return new TimeDto(sumH, sumM);
    }
    
    public static TimeDto operator -(TimeDto a, TimeDto b)
    {
        var sumH = b.Hour - a.Hour;
        var sumM = b.Minutes - a.Minutes;
        if (sumM < 0)
        {
            sumM += 60;
            sumH -= 1;
        }

        if (sumH < 0)
            throw new ArgumentException();
        return new TimeDto(sumH, sumM);
    }

    public static decimal ConvertToDecimal(TimeDto time)
    {
        return time.Hour + (decimal)time.Minutes / 60;
    }

    public static TimeDto ConvertToTime(decimal time)
    {
        return new TimeDto((int)time, (int)((time - (int)time) * 60));
    }

    public static int CompareTo(TimeDto a, TimeDto b)
    {
        var res1 = a.Hour.CompareTo(b.Hour);
        return res1 != 0 ? res1 : a.Minutes.CompareTo(b.Minutes);
    }
}