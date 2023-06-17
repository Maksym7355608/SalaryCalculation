using SalaryCalculation.Data.BaseEventModels;

namespace SalaryCalculation.Data;

public interface IMessageBus
{
    Task PublishEventAsync<M>(M msg) where M : BusEvent;
    
    Task PublishEventAsync<TBase, T>(T msg)
        where TBase : BusEvent
        where T : TBase;
    
    Task PublishMessageAsync<M>(M msg) where M : BusMessage;
    
    Task PublishMessageAsync<TBase, T>(T msg)
        where TBase : BusMessage
        where T : TBase;
}