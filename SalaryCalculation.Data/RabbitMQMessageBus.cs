using Newtonsoft.Json;
using SalaryCalculation.Data.BaseModels;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace SalaryCalculation.Data;

public class RabbitMQMessageBus : IMessageBus
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly Dictionary<Type, List<Action<BusEvent>>> _subscribers;

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
    
    public void Subscribe<T>(Action<T> handler) where T : BusEvent
    {
        var exchangeName = typeof(T).Name;
        var queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var msg = JsonConvert.DeserializeObject<T>(message);
            handler.Invoke(msg);
        };

        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        if (_subscribers.ContainsKey(typeof(T)))
        {
            _subscribers[typeof(T)].Add((Action<BusEvent>)(Delegate)handler);
        }
        else
        {
            _subscribers[typeof(T)] = new List<Action<BusEvent>> { (Action<BusEvent>)(Delegate)handler };
        }
    }


    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}