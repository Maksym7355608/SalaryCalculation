namespace Schedule.Data.BaseModels;

public class Time
{
    public string TimeValue => $"{Hour}:{Minutes}";
    public int Hour { get; set; }
    public int Minutes { get; set; }
    
    public Time(int hour, int minutes)
    {
        Hour = hour;
        Minutes = minutes;
    }
    
    public static bool operator >(Time a, Time b)
    {
        return CompareTo(a, b) > 0;
    }

    public static bool operator <(Time a, Time b)
    {
        return CompareTo(a, b) < 0;
    }
    
    public static bool operator >=(Time a, Time b)
    {
        return CompareTo(a, b) >= 0;
    }

    public static bool operator <=(Time a, Time b)
    {
        return CompareTo(a, b) <= 0;
    }
    
    public static bool operator !=(Time a, Time b)
    {
        return CompareTo(a, b) != 0;
    }
    
    public static bool operator ==(Time a, Time b)
    {
        return CompareTo(a, b) == 0;
    }

    public static Time operator +(Time a, Time b)
    {
        var sumH = a.Hour + b.Hour + (a.Minutes + b.Minutes) / 60;
        var sumM = (a.Minutes + b.Minutes) % 60;
        return new Time(sumH, sumM);
    }
    
    public static Time operator -(Time a, Time b)
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
        return new Time(sumH, sumM);
    }

    public static decimal ConvertToDecimal(Time time)
    {
        return time.Hour + (decimal)time.Minutes / 60;
    }

    public static Time ConvertToTime(decimal time)
    {
        return new Time((int)time, (int)((time - (int)time) * 60));
    }

    public static int CompareTo(Time a, Time b)
    {
        var res1 = a.Hour.CompareTo(b.Hour);
        return res1 != 0 ? res1 : a.Minutes.CompareTo(b.Minutes);
    }
}