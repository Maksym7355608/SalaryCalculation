using Dictionary.App.Abstract;
using Dictionary.App.CommandHandlers;
using Organization.Data.Data;
using SalaryCalculation.Data;
using SalaryCalculation.Data.Infrastructure;

namespace Dictionary.API;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDictionaryUnitOfWork(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetValue<string>("MongoDbConnectionString");
        var dbName = configuration.GetValue<string>("DictionaryDbName");
        return services.AddTransient(provider => {
            var bus = provider.GetService<IMessageBroker>();
            IUnitOfWork work = new UnitOfWork(cs, dbName, bus);

            return work;
        });
    }
    
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<IDictionaryCommandHandler, DictionaryCommandHandler>();
        return services;
    }
}