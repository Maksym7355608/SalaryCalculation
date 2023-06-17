namespace SalaryCalculation.Data.BaseEventModels;

public class BusEvent
{
    public Guid EventId { get; set; }
    public DateTime EventTime { get; set; }

    public BusEvent()
    {
        EventId = Guid.NewGuid();
        EventTime = DateTime.UtcNow;
    }
}