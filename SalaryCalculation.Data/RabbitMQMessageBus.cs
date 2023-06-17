using Newtonsoft.Json;
using SalaryCalculation.Data.BaseEventModels;

namespace SalaryCalculation.Data;

using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

public class RabbitMQMessageBus : IMessageBus
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQMessageBus(string connectionString)
    {
        var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public async Task PublishEventAsync<M>(M msg) where M : BusEvent
    {
        var exchangeName = typeof(M).Name;
        var routingKey = "";
        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));

        _channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);
    }

    public async Task PublishEventAsync<TBase, T>(T msg)
        where TBase : BusEvent
        where T : TBase
    {
        await PublishEventAsync<T>(msg);
    }

    public async Task PublishMessageAsync<M>(M msg) where M : BusMessage
    {
        var exchangeName = typeof(M).Name;
        var routingKey = "";
        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));

        _channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);
    }

    public async Task PublishMessageAsync<TBase, T>(T msg)
        where TBase : BusMessage
        where T : TBase
    {
        await PublishMessageAsync<T>(msg);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}