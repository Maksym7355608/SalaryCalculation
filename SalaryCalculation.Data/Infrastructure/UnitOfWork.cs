using MongoDB.Driver;

namespace SalaryCalculation.Data.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    protected readonly IMongoDatabase Database;

    public IMessageBroker MessageBroker { get; }

    public UnitOfWork(string connectionString, IMessageBroker broker) : this(new MongoUrl(connectionString), broker)
    { }

    public UnitOfWork(MongoUrl connectionUrl, IMessageBroker broker)
    {
        MessageBroker = broker;
        var client = new MongoClient(connectionUrl);
        Database = client.GetDatabase(connectionUrl.DatabaseName);
    }
    
    public IMongoCollection<T> GetCollection<T>()
    {
        return Database.GetCollection<T>(typeof(T).Name);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return Database.GetCollection<T>(name);
    }
}