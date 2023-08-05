using AutoMapper;
using Microsoft.Extensions.Logging;
using Organization.App.Abstract;
using Organization.App.Commands;
using Organization.App.DtoModels;
using Organization.Data.Data;

namespace Organization.App.Handlers;

public class OrganizationCommandHandler : BaseOrganizationCommandHandler, IOrganizationCommandHandler
{
    public OrganizationCommandHandler(IOrganizationUnitOfWork work, ILogger logger, IMapper mapper) : base(work, logger, mapper)
    {
    }

    public async Task<OrganizationDto> GetOrganizationAsync(int organizationId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<OrganizationUnitDto>> SearchOrganizationUnitsAsync(OrganizationUnitSearchCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<PositionDto>> SearchPositionsAsync(PositionSearchCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task CreateOrganizationAsync(OrganizationCreateCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateOrganizationAsync(OrganizationUpdateCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteOrganizationAsync(int organizationId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateOrganizationPermissionsAsync(OrganizationPermissionUpdateCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task CreateOrganizationUnitAsync(OrganizationUnitCreateCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateOrganizationUnitAsync(OrganizationUnitUpdateCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteOrganizationUnitAsync(int organizationId, int organizationUnitId)
    {
        throw new NotImplementedException();
    }

    public async Task CreatePositionAsync(PositionCreateCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdatePositionAsync(PositionUpdateCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeletePositionAsync(int organizationId, int organizationUnitId, int positionId)
    {
        throw new NotImplementedException();
    }
}