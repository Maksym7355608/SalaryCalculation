using System.Data;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Organization.App.Abstract;
using Organization.App.Commands;
using Organization.App.DtoModels;
using Organization.Data.Data;
using Organization.Data.Entities;
using SalaryCalculation.Data.Enums;
using SalaryCalculation.Shared.Common.Validation;
using Org = Organization.Data.Entities.Organization;

namespace Organization.App.Handlers;

public class OrganizationCommandHandler : BaseOrganizationCommandHandler, IOrganizationCommandHandler
{
    public OrganizationCommandHandler(IOrganizationUnitOfWork work, ILogger<OrganizationCommandHandler> logger, IMapper mapper) : base(work, logger, mapper)
    {
    }

    public async Task<OrganizationDto> GetOrganizationAsync(int organizationId)
    {
        var organizationTask = Work.GetCollection<Org>(nameof(Org))
            .Find(x => x.Id == organizationId)
            .FirstOrDefaultAsync();
        var organizationPermissionsTask = Work.GetCollection<OrganizationPermissions>(nameof(OrganizationPermissions))
            .Find(x => x.OrganizationId == organizationId)
            .Project(x => x.Permissions)
            .FirstOrDefaultAsync();
        await Task.WhenAll(organizationTask, organizationPermissionsTask);

        var organization = organizationTask.Result;
        var organizationPermissions = organizationPermissionsTask.Result;

        if (organization == null || organizationPermissions == null)
            throw new EntityNotFoundException(organizationId.ToString());

        var dto = Mapper.Map<OrganizationDto>(organization);
        dto.Permissions = organizationPermissions.Select(x => (EPermission)x);
        return dto;
    }

    public async Task<IEnumerable<OrganizationUnitDto>> SearchOrganizationUnitsAsync(OrganizationUnitSearchCommand command)
    {
        var filter = GetOrganizationUnitSearchFilter(command);
        var orgUnits = await Work.GetCollection<OrganizationUnit>()
            .Find(filter)
            .ToListAsync();
        return Mapper.Map<IEnumerable<OrganizationUnitDto>>(orgUnits);
    }

    private FilterDefinition<OrganizationUnit> GetOrganizationUnitSearchFilter(OrganizationUnitSearchCommand command)
    {
        var filterBuilder = Builders<OrganizationUnit>.Filter;
        var filterDefinition = new List<FilterDefinition<OrganizationUnit>>()
        {
            filterBuilder.Eq(x => x.OrganizationId, command.OrganizationId)
        };
        if(!string.IsNullOrWhiteSpace(command.Name))
            filterDefinition.Add(filterBuilder.Regex(x => x.Name, new BsonRegularExpression(command.Name, "i")));
        return filterBuilder.And(filterDefinition);
    }

    public async Task<IEnumerable<PositionDto>> SearchPositionsAsync(PositionSearchCommand command)
    {
        var filter = GetPositionSearchFilter(command);
        var positions = await Work.GetCollection<Position>()
            .Find(filter)
            .ToListAsync();
        return Mapper.Map<IEnumerable<PositionDto>>(positions);
    }

    private FilterDefinition<Position> GetPositionSearchFilter(PositionSearchCommand command)
    {
        var filterBuilder = Builders<Position>.Filter;
        var filterDefinition = new List<FilterDefinition<Position>>()
        {
            filterBuilder.Eq(x => x.OrganizationId, command.OrganizationId),
            filterBuilder.Eq(x => x.OrganizationUnitId, command.OrganizationUnitId)
        };
        if(!string.IsNullOrWhiteSpace(command.Name))
            filterDefinition.Add(filterBuilder.Regex(x => x.Name, new BsonRegularExpression(command.Name, "i")));
        return filterBuilder.And(filterDefinition);
    }

    public async Task CreateOrganizationAsync(OrganizationCreateCommand command)
    {
        var organization = Mapper.Map<Org>(command);
        if (Work.GetCollection<Org>(nameof(Org)).Find(x => x.Code == organization.Code)
            .Any())
            throw new DuplicateNameException("Organization with the same code exist");
        await Work.GetCollection<Org>(nameof(Org))
            .InsertOneAsync(organization);
    }

    public async Task<bool> UpdateOrganizationAsync(OrganizationUpdateCommand command)
    {
        var organization = Mapper.Map<Org>(command);
        if (!Work.GetCollection<Org>(nameof(Org)).Find(x => x.Code == organization.Code)
            .Any())
            throw new EntityNotFoundException(organization.Code);
        var result = await Work.GetCollection<Org>(nameof(Org))
            .UpdateOneAsync(x => x.Code == organization.Code, Builders<Org>.Update
                .Set(x => x, organization));
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteOrganizationAsync(int organizationId)
    {
        var result = await Work.GetCollection<Org>(nameof(Org)).DeleteOneAsync(x => x.Id == organizationId);
        return result.DeletedCount > 0;
    }

    public async Task<bool> UpdateOrganizationPermissionsAsync(OrganizationPermissionUpdateCommand command)
    {
        bool result;
        if (!Work.GetCollection<Org>(nameof(Org)).Find(x => x.Id == command.OrganizationId)
                .Any())
            throw new EntityNotFoundException(command.OrganizationId.ToString());
        if (Work.GetCollection<OrganizationPermissions>().Find(x => x.OrganizationId == command.OrganizationId)
            .Any())
            result = (await Work.GetCollection<OrganizationPermissions>()
                .UpdateOneAsync(x => x.OrganizationId == command.OrganizationId,
                    Builders<OrganizationPermissions>.Update.Set(x => x.Permissions,
                        command.Permissions.Cast<int>()))).ModifiedCount > 0;
        else
        {
            await Work.GetCollection<OrganizationPermissions>()
                .InsertOneAsync(Mapper.Map<OrganizationPermissions>(command));
            result = true;
        }

        return result;
    }

    public async Task CreateOrganizationUnitAsync(OrganizationUnitCreateCommand command)
    {
        var organizationUnit = Mapper.Map<OrganizationUnit>(command);
        if (Work.GetCollection<OrganizationUnit>(nameof(OrganizationUnit)).Find(x => x.Name == organizationUnit.Name)
            .Any())
            throw new DuplicateNameException("Organization unit with the same name exist");
        await Work.GetCollection<OrganizationUnit>(nameof(OrganizationUnit))
            .InsertOneAsync(organizationUnit);
    }

    public async Task<bool> UpdateOrganizationUnitAsync(OrganizationUnitUpdateCommand command)
    {
        var organizationUnit = Mapper.Map<OrganizationUnit>(command);
        if (!Work.GetCollection<OrganizationUnit>(nameof(OrganizationUnit)).Find(x => x.Name == organizationUnit.Name)
                .Any())
            throw new EntityNotFoundException(organizationUnit.Name);
        var result = await Work.GetCollection<OrganizationUnit>(nameof(OrganizationUnit))
            .UpdateOneAsync(x => x.Name == organizationUnit.Name, Builders<OrganizationUnit>.Update
                .Set(x => x, organizationUnit));
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteOrganizationUnitAsync(int organizationId, int organizationUnitId)
    {
        var result = await Work.GetCollection<OrganizationUnit>(nameof(OrganizationUnit))
            .DeleteOneAsync(x => x.OrganizationId == organizationId && x.Id == organizationUnitId);
        return result.DeletedCount > 0;
    }

    public async Task CreatePositionAsync(PositionCreateCommand command)
    {
        var position = Mapper.Map<Position>(command);
        if (Work.GetCollection<Position>(nameof(Position)).Find(x => x.Name == position.Name)
            .Any())
            throw new DuplicateNameException("Position with the same name exist");
        await Work.GetCollection<Position>(nameof(Position))
            .InsertOneAsync(position);
    }

    public async Task<bool> UpdatePositionAsync(PositionUpdateCommand command)
    {
        var organization = Mapper.Map<Position>(command);
        if (!Work.GetCollection<Position>(nameof(Position)).Find(x => x.Name == organization.Name)
                .Any())
            throw new EntityNotFoundException(organization.Name);
        var result = await Work.GetCollection<Position>(nameof(Position))
            .UpdateOneAsync(x => x.Name == organization.Name, Builders<Position>.Update
                .Set(x => x, organization));
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeletePositionAsync(int organizationId, int organizationUnitId, int positionId)
    {
        var result = await Work.GetCollection<Position>(nameof(Position)).DeleteOneAsync(x =>
            x.OrganizationId == organizationId && x.OrganizationUnitId == organizationUnitId && x.Id == positionId);
        return result.DeletedCount > 0;
    }

    public async Task<IEnumerable<OrganizationDto>> GetOrganizationsAsync()
    {
        return Mapper.Map<IEnumerable<OrganizationDto>>(await Work.GetCollection<Org>(nameof(Org)).Find(Builders<Org>.Filter.Empty).ToListAsync());
    }

    public async Task<OrganizationUnitDto> GetOrganizationUnitAsync(int organizationId, int id)
    {
        var orgUnit = await Work.GetCollection<OrganizationUnit>(nameof(OrganizationUnit))
            .Find(x => x.OrganizationId == organizationId && x.Id == id).FirstOrDefaultAsync();
        if (orgUnit == null)
            throw new EntityNotFoundException(id.ToString());
        return Mapper.Map<OrganizationUnitDto>(orgUnit);
    }

    public async Task<PositionDto> GetPositionAsync(int organizationId, int organizationUnitId, int id)
    {
        var orgUnit = await Work.GetCollection<Position>(nameof(Position))
            .Find(x => x.OrganizationId == organizationId && x.OrganizationUnitId == organizationUnitId && x.Id == id).FirstOrDefaultAsync();
        if (orgUnit == null)
            throw new EntityNotFoundException(id.ToString());
        return Mapper.Map<PositionDto>(orgUnit);
    }
}