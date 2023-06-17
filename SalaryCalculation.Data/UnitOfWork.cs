using MongoDB.Driver;
using SalaryCalculation.Data.BaseModels;

namespace SalaryCalculation.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly IMongoDatabase _database;
    public IMessageBus Bus { get; }

    public UnitOfWork(IMongoDatabase database, IMessageBus bus)
    {
        _database = database;
        Bus = bus;
    }

    public IMongoCollection<T> GetCollection<T>()
    {
        return _database.GetCollection<T>(typeof(T).Name);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }

    public Task PublishEventAsync<M>(M msg) where M : BusEvent
    {
        return Bus.PublishEventAsync(msg);
    }

    public Task PublishEventAsync<TBase, T>(T msg)
        where TBase : BusEvent
        where T : TBase
    {
        return Bus.PublishEventAsync<TBase, T>(msg);
    }
}