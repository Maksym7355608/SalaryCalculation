using Calculation.App.Abstract;
using Calculation.App.Handlers;
using Organization.Data.Data;
using SalaryCalculation.Data;
using Schedule.Data.Data;

namespace Calculation.API;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoOrganizationUnitOfWork(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetValue<string>("MongoDbConnectionString");
        var dbName = configuration.GetValue<string>("OrganizationDbName");
        return services.AddTransient(provider => {
            var bus = provider.GetService<IMessageBroker>();
            IOrganizationUnitOfWork work = new OrganizationUnitOfWork(cs, dbName, bus);

            return work;
        });
    }
    
    public static IServiceCollection AddCalculationUnitOfWork(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetValue<string>("MongoDbConnectionString");
        var dbName = configuration.GetValue<string>("CalculationDbName");
        return services.AddTransient(provider => {
            var bus = provider.GetService<IMessageBroker>();
            IOrganizationUnitOfWork work = new OrganizationUnitOfWork(cs, dbName, bus);

            return work;
        });
    }
    
    public static IServiceCollection AddMongoScheduleUnitOfWork(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetValue<string>("MongoDbConnectionString");
        var dbName = configuration.GetValue<string>("ScheduleDbName");
        return services.AddTransient(provider => {
            var bus = provider.GetService<IMessageBroker>();
            IScheduleUnitOfWork work = new ScheduleUnitOfWork(cs, dbName, bus);

            return work;
        });
    }
    
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<IOperationCommandHandler, OperationCommandHandler>();
        services.AddScoped<IPaymentCardCommandHandler, PaymentCardCommandHandler>();
        return services;
    }
}