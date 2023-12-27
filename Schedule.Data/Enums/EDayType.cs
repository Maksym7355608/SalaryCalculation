using System.ComponentModel;

namespace Schedule.Data.Enums;

public enum EDayType
{
    [Description("-")]
    Absence = 0,
    
    Work = 1,
    [Description("C")]
    Holiday = 2,
    [Description("Х")]
    Sick = 3,
    [Description("В")]
    Vacation = 4,
}