using SalaryCalculation.Data.BaseModels;

namespace SalaryCalculation.Data;

public interface IMessageBus
{
    Task PublishEventAsync<M>(M msg) where M : BusEvent;
    
    Task PublishEventAsync<TBase, T>(T msg)
        where TBase : BusEvent
        where T : TBase;
    
    
}