using SalaryCalculation.Data.BaseModels;

namespace SalaryCalculation.Data;

public interface IMessageBroker
{
    Task PublishAsync<TMessage>(TMessage message) where TMessage : BaseMessage;
    Task SubscribeAsync<TMessage>(string subscriptionId, Func<TMessage, Task> handler) where TMessage : BaseMessage;
}