using Organization.Data.Data;
using SalaryCalculation.Data;
using Schedule.App.Abstract;
using Schedule.App.Handlers;
using Schedule.Data.Data;

namespace Schedule.API;

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
        services.AddScoped<IScheduleCommandHandler, ScheduleCommandHandler>();
        services.AddScoped<IScheduleReaderLogic, ScheduleReaderLogic>();
        return services;
    }
}