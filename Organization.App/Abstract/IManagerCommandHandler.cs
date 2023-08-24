using Organization.App.Commands;

namespace Organization.App.Abstract;

public interface IManagerCommandHandler
{
    Task<bool> AddManagerToOrganizationAsync(ManagerAddCommand command);
    Task<bool> RemoveManagerFromOrganizationAsync(int organizationId);
}