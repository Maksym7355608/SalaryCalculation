using Schedule.Data.Enums;

namespace Schedule.App.Dto;

public class EmpDayDto
{
    public int EmployeeId { get; set; }
    public string Date { get; set; }
    public decimal? Day { get; set; }
    public decimal? Evening { get; set; }
    public decimal? Night { get; set; }
    public string Summary { get; set; }
    public bool? Holiday { get; set; }
}

public class EmpDayShortDto
{
    public string Day { get; set; }
    public string Work { get; set; }
}