using System.ComponentModel;

namespace Schedule.Data.Enums;

public enum EDayType
{
    [Description("-")]
    Absence = 0,
    
    Work = 1,
    [Description("H")]
    Holiday = 2,
    [Description("L")]
    Sick = 3,
    [Description("V")]
    Vacation = 4,
}