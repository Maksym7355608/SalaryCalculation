using AutoMapper;
using Identity.App.Abstract;
using Identity.Data.Data;
using SalaryCalculation.Data;
using SalaryCalculation.Data.BaseHandlers;

namespace Identity.App.Handlers;

public class BaseIdentityCommandHandler : BaseCommandHandler
{
    public BaseIdentityCommandHandler(IIdentityUnitOfWork work, IMapper mapper) : base(work, mapper)
    { }
}