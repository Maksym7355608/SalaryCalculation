using MongoDB.Driver;
using SalaryCalculation.Data.BaseModels;

namespace SalaryCalculation.Data;

public interface IUnitOfWork
{
    IMessageBroker MessageBroker { get; }
    IMongoCollection<T> GetCollection<T>();
    IMongoCollection<T> GetCollection<T>(string name);
    Task<K> NextValue<T, K>(int reservationCount = 1)
        where T : BaseMongoDomainModel
        where K : struct;
}