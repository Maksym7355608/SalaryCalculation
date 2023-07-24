using AutoMapper;
using Microsoft.Extensions.Logging;
using SalaryCalculation.Shared.Common.Validation;

namespace SalaryCalculation.Data.BaseHandlers;

public abstract class BaseCommandHandler
{
    protected IMapper Mapper { get; }
    protected IUnitOfWork Work { get; }
    protected ILogger Logger { get; }

    public BaseCommandHandler(IUnitOfWork work, ILogger logger, IMapper mapper)
    {
        Work = work;
        Mapper = mapper;
        Logger = logger;
    }
    
    protected void CheckIfNotFound<T>(int id, T? entity) where T : class
    {
        if (entity != null)
            return;
        ThrowNotFound<T>(id);
    }

    protected void CheckIfNotFound<T>(int id, object? entity) where T : class
    {
        if (entity != null)
            return;
        ThrowNotFound<T>(id);
    }

    protected void ThrowNotFound<T>(int id) =>
        throw new EntityNotFoundException("Entity {0} with id {1} was not found", typeof(T).Name, id.ToString());
}