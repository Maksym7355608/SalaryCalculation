using EasyNetQ;
using SalaryCalculation.Data.BaseModels;

namespace SalaryCalculation.Data.Infrastructure;

public class RabbitMqMessageBroker : IMessageBroker
{
    
        public RabbitMqMessageBroker(string connectionString)
        {
            Bus = RabbitHutch.CreateBus(connectionString);
        }

        public IBus Bus { get; set; }

        public Task PublishAsync<T>(T message) where T : BaseMessage
        {
            return Bus.PubSub.PublishAsync(message);
        }

        public Task SubscribeAsync<T>(string subscriptionId, Func<T, Task> handler) where T : BaseMessage
        {
            Bus.PubSub.SubscribeAsync(subscriptionId, handler);
            return Task.CompletedTask;
        }
    
}