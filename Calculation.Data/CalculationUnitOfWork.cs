using MongoDB.Driver;
using SalaryCalculation.Data;
using SalaryCalculation.Data.Infrastructure;

namespace Calculation.Data;

public class CalculationUnitOfWork : UnitOfWork, ICalculationUnitOfWork
{
    public CalculationUnitOfWork(string connectionString, string databaseName, IMessageBroker broker) : base(connectionString, databaseName, broker)
    {
    }

    public CalculationUnitOfWork(MongoUrl connectionUrl, string databaseName, IMessageBroker broker) : base(connectionUrl, databaseName, broker)
    {
    }
}