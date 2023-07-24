using MongoDB.Driver;
using SalaryCalculation.Data.BaseModels;

namespace SalaryCalculation.Data;

public interface IUnitOfWork
{
    IMessageBroker MessageBroker { get; }
    IMongoCollection<T> GetCollection<T>();
    IMongoCollection<T> GetCollection<T>(string name);
}