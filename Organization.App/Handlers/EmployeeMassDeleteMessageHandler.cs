using AutoMapper;
using Microsoft.Extensions.Logging;
using Organization.App.Commands.Messages;
using Organization.Data.Data;

namespace Organization.App.Handlers;

public class EmployeeMassDeleteMessageHandler : BaseOrganizationMessageHandler<EmployeeMassDeleteMessage>
{
    public EmployeeMassDeleteMessageHandler(IOrganizationUnitOfWork work, ILogger logger, IMapper mapper) : base(work, logger, mapper)
    {
    }

    public override async Task HandleAsync(EmployeeMassDeleteMessage msg)
    {
        throw new NotImplementedException();
    }
}