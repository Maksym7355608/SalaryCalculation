using AutoMapper;
using Microsoft.Extensions.Logging;
using Organization.App.Commands.Messages;
using Organization.Data.Data;

namespace Organization.App.Handlers;

public class EmployeeMassUpdateMessageHandler : BaseOrganizationMessageHandler<EmployeeMassUpdateMessage>
{
    public EmployeeMassUpdateMessageHandler(IOrganizationUnitOfWork work, ILogger<EmployeeMassUpdateMessageHandler> logger, IMapper mapper) : base(work, logger, mapper)
    {
    }

    public override async Task HandleAsync(EmployeeMassUpdateMessage msg)
    {
        throw new NotImplementedException();
    }
}