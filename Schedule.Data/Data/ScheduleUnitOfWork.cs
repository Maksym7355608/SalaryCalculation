using MongoDB.Driver;
using SalaryCalculation.Data;
using SalaryCalculation.Data.Infrastructure;

namespace Schedule.Data.Data;

public class ScheduleUnitOfWork : UnitOfWork, IScheduleUnitOfWork
{
    public ScheduleUnitOfWork(string connectionString, string databaseName, IMessageBroker broker) : base(connectionString, databaseName, broker)
    {
    }

    public ScheduleUnitOfWork(MongoUrl connectionUrl, string databaseName, IMessageBroker broker) : base(connectionUrl, databaseName, broker)
    {
    }
}