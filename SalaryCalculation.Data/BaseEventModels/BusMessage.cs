namespace SalaryCalculation.Data.BaseEventModels;

public class BusMessage
{
    public string MessageType { get; set; }
    public object Data { get; set; }

    public BusMessage(string messageType, object data)
    {
        MessageType = messageType;
        Data = data;
    }
}