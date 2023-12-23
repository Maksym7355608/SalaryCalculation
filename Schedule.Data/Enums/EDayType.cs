using System.ComponentModel;

namespace Schedule.Data.Enums;

public enum EDayType
{
    [Description("-")]
    Absence = 0,
    
    Work = 1,
    [Description("В")]
    Holiday = 2,
    [Description("Х")]
    Sick = 3,
}