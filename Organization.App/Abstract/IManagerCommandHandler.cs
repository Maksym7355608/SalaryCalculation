using Organization.App.Commands;

namespace Organization.App.Abstract;

public interface IManagerCommandHandler
{
    Task AddManagerToOrganizationAsync(ManagerAddCommand command);
    Task RemoveManagerFromOrganizationAsync(int organizationId);
}