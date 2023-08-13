using AutoMapper;
using Microsoft.Extensions.Logging;
using Organization.App.Commands.Messages;
using Organization.Data.Data;
using SalaryCalculation.Data;
using SalaryCalculation.Data.BaseHandlers;

namespace Organization.App.Handlers;

public class EmployeeMassCreateMessageHandler : BaseOrganizationMessageHandler<EmployeeMassCreateMessage>
{
    public EmployeeMassCreateMessageHandler(IOrganizationUnitOfWork work, ILogger logger, IMapper mapper) : base(work, logger, mapper)
    {
    }

    public override async Task HandleAsync(EmployeeMassCreateMessage msg)
    {
        throw new NotImplementedException();
    }
}