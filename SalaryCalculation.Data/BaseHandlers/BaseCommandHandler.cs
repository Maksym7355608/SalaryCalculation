using AutoMapper;

namespace SalaryCalculation.Data.BaseHandlers;

public abstract class BaseCommandHandler
{
    protected IMapper Mapper;
    protected IUnitOfWork Work;

    public BaseCommandHandler(IUnitOfWork work, IMapper mapper)
    {
        Work = work;
        Mapper = mapper;
    }
}