using AutoMapper;
using Microsoft.Extensions.Logging;
using Organization.App.Abstract;
using Organization.App.Commands;
using Organization.Data.Data;

namespace Organization.App.Handlers;

public class ManagerCommandHandler : BaseOrganizationCommandHandler, IManagerCommandHandler
{
    public ManagerCommandHandler(IOrganizationUnitOfWork work, ILogger logger, IMapper mapper) : base(work, logger, mapper)
    {
    }

    public async Task AddManagerToOrganizationAsync(ManagerAddCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveManagerFromOrganizationAsync(int organizationId)
    {
        throw new NotImplementedException();
    }
}