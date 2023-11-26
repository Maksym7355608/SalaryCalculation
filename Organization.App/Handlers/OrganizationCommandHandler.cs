using System.Data;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Organization.App.Abstract;
using Organization.App.Commands;
using Organization.App.DtoModels;
using Organization.Data.BaseModels;
using Organization.Data.Data;
using Organization.Data.Entities;
using SalaryCalculation.Data.BaseModels;
using SalaryCalculation.Data.Enums;
using SalaryCalculation.Shared.Common.Validation;
using Org = Organization.Data.Entities.Organization;

namespace Organization.App.Handlers;

public class OrganizationCommandHandler : BaseOrganizationCommandHandler, IOrganizationCommandHandler
{
    private readonly int[] _basePermissionsList = Enum.GetValues<EPermission>().Cast<int>().ToArray();
    public OrganizationCommandHandler(IOrganizationUnitOfWork work, ILogger<OrganizationCommandHandler> logger, IMapper mapper) : base(work, logger, mapper)
    {
    }

    public async Task<OrganizationDto> GetOrganizationAsync(int organizationId)
    {
        var organizationTask = Work.GetCollection<Org>(nameof(Organization.Data.Entities.Organization))
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
        dto.Permissions = organizationPermissions.Cast<EPermission>();
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
        var organization = new Org()
        {
            Id = (int)Work.GetCollection<Org>(nameof(Organization.Data.Entities.Organization)).Find(Builders<Org>.Filter.Empty).CountDocuments(),
            Code = command.Code,
            Name = command.Name,
            Address = command.Address,
            FactAddress = command.FactAddress,
            Accountant = command.Accountant,
            BankAccounts = Mapper.Map<IEnumerable<Bank>>(command.BankAccounts),
            Chief = command.Chief,
            Edrpou = command.Edrpou
        };
        if (Work.GetCollection<Org>(nameof(Organization.Data.Entities.Organization)).Find(x => x.Code == organization.Code)
            .Any())
            throw new DuplicateNameException("Organization with the same code exist");
        await Work.GetCollection<Org>(nameof(Organization.Data.Entities.Organization))
            .InsertOneAsync(organization);
        var organizationPermissions = new OrganizationPermissions()
        {
            OrganizationId = organization.Id,
            Permissions = _basePermissionsList,
        };
        await Work.GetCollection<OrganizationPermissions>()
            .InsertOneAsync(organizationPermissions);
    }

    public async Task<bool> UpdateOrganizationAsync(OrganizationUpdateCommand command)
    {
        var organization = new Org()
        {
            Id = command.Id,
            Code = command.Code,
            Name = command.Name,
            Address = command.Address,
            FactAddress = command.FactAddress,
            Accountant = command.Accountant,
            BankAccounts = Mapper.Map<IEnumerable<Bank>>(command.BankAccounts),
            Chief = command.Chief,
            Edrpou = command.Edrpou,
        };
        if (!Work.GetCollection<Org>(nameof(Organization.Data.Entities.Organization)).Find(x => x.Code == organization.Code)
            .Any())
            throw new EntityNotFoundException(organization.Code);
        var result = await Work.GetCollection<Org>(nameof(Organization.Data.Entities.Organization))
            .ReplaceOneAsync(x => x.Id == organization.Id, organization);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteOrganizationAsync(int organizationId)
    {
        var result = await Work.GetCollection<Org>(nameof(Organization.Data.Entities.Organization)).DeleteOneAsync(x => x.Id == organizationId);
        await Work.GetCollection<OrganizationPermissions>()
            .DeleteOneAsync(x => x.OrganizationId == organizationId);
        await Work.GetCollection<OrganizationUnit>()
            .DeleteManyAsync(x => x.OrganizationId == organizationId);
        await Work.GetCollection<Position>()
            .DeleteManyAsync(x => x.OrganizationId == organizationId);
        await Work.GetCollection<Employee>()
            .DeleteManyAsync(x => x.Organization.Id == organizationId);
        return result.DeletedCount > 0;
    }

    public async Task<bool> UpdateOrganizationPermissionsAsync(OrganizationPermissionUpdateCommand command)
    {
        bool result;
        if (!Work.GetCollection<Org>(nameof(Organization.Data.Entities.Organization)).Find(x => x.Id == command.OrganizationId)
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
        var lastId = Work.GetCollection<OrganizationUnit>(nameof(OrganizationUnit)).Find(Builders<OrganizationUnit>.Filter.Empty)
            .CountDocuments();
        organizationUnit.Id = (int)lastId;
        if (Work.GetCollection<OrganizationUnit>(nameof(OrganizationUnit)).Find(x => x.Name == organizationUnit.Name)
            .Any())
            throw new DuplicateNameException("Organization unit with the same name exist");
        await Work.GetCollection<OrganizationUnit>(nameof(OrganizationUnit))
            .InsertOneAsync(organizationUnit);
    }

    public async Task<bool> UpdateOrganizationUnitAsync(OrganizationUnitUpdateCommand command)
    {
        var organizationUnit = Mapper.Map<OrganizationUnit>(command);
        if (!Work.GetCollection<OrganizationUnit>(nameof(OrganizationUnit)).Find(x => x.Id == organizationUnit.Id)
                .Any())
            throw new EntityNotFoundException(organizationUnit.Name);
        var result = await Work.GetCollection<OrganizationUnit>(nameof(OrganizationUnit))
            .UpdateOneAsync(x => x.Id == organizationUnit.Id, Builders<OrganizationUnit>.Update
                .Set(x => x.Name, organizationUnit.Name)
                .Set(x => x.OrganizationUnitId, organizationUnit.OrganizationUnitId));
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteOrganizationUnitAsync(int organizationId, int organizationUnitId)
    {
        var result = await Work.GetCollection<OrganizationUnit>(nameof(OrganizationUnit))
            .DeleteOneAsync(x => x.OrganizationId == organizationId && x.Id == organizationUnitId);
        await Work.GetCollection<OrganizationUnit>()
            .UpdateManyAsync(x => x.OrganizationUnitId == organizationUnitId,
                Builders<OrganizationUnit>.Update.Set(x => x.OrganizationUnitId, null));
        await Work.GetCollection<Position>()
            .DeleteManyAsync(x => x.OrganizationId == organizationId && x.OrganizationUnitId == organizationUnitId);
        await Work.GetCollection<Employee>()
            .DeleteManyAsync(x => x.Organization.Id == organizationId && x.OrganizationUnit.Id == organizationUnitId);
        return result.DeletedCount > 0;
    }

    public async Task CreatePositionAsync(PositionCreateCommand command)
    {
        var position = Mapper.Map<Position>(command);
        var lastId = Work.GetCollection<Position>().Find(Builders<Position>.Filter.Empty)
            .CountDocuments();
        position.Id = (int)lastId;
        if (Work.GetCollection<Position>(nameof(Position)).Find(x => x.Name == position.Name)
            .Any())
            throw new DuplicateNameException("Position with the same name exist");
        await Work.GetCollection<Position>(nameof(Position))
            .InsertOneAsync(position);
    }

    public async Task<bool> UpdatePositionAsync(PositionUpdateCommand command)
    {
        var position = Mapper.Map<Position>(command);
        if (!Work.GetCollection<Position>(nameof(Position)).Find(x => x.Id == position.Id)
                .Any())
            throw new EntityNotFoundException(position.Name);
        var result = await Work.GetCollection<Position>(nameof(Position))
            .UpdateOneAsync(x => x.Id == position.Id, Builders<Position>.Update
                .Set(x => x.Name, position.Name)
                .Set(x => x.OrganizationUnitId, position.OrganizationUnitId));
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeletePositionAsync(int organizationId, int organizationUnitId, int positionId)
    {
        var result = await Work.GetCollection<Position>(nameof(Position)).DeleteOneAsync(x =>
            x.OrganizationId == organizationId && x.OrganizationUnitId == organizationUnitId && x.Id == positionId);
        await Work.GetCollection<Employee>().DeleteManyAsync(x => x.Position.Id == positionId);
        return result.DeletedCount > 0;
    }

    public async Task<IEnumerable<OrganizationDto>> GetOrganizationsAsync()
    {
        return Mapper.Map<IEnumerable<OrganizationDto>>(await Work.GetCollection<Org>(nameof(Organization.Data.Entities.Organization)).Find(Builders<Org>.Filter.Empty).ToListAsync());
    }
    
    public async Task<IEnumerable<IdNamePair>> GetOrganizationsShortAsync()
    {
        var result = await Work.GetCollection<Org>(nameof(Organization.Data.Entities.Organization)).Find(Builders<Org>.Filter.Empty)
            .Project(org => new { org.Id, org.Name }).ToListAsync();
        return result.Select(x => new IdNamePair(x.Id, x.Name));
    }

    public async Task<OrganizationUnitDto> GetOrganizationUnitAsync(int organizationId, int id)
    {
        var orgUnit = await Work.GetCollection<OrganizationUnit>(nameof(OrganizationUnit))
            .Find(x => x.OrganizationId == organizationId && x.Id == id).FirstOrDefaultAsync();
        if (orgUnit == null)
            throw new EntityNotFoundException(id.ToString());
        return Mapper.Map<OrganizationUnitDto>(orgUnit);
    }

    public async Task<IEnumerable<OrganizationUnitDto>> GetOrganizationUnitsAsync(int organizationId)
    {
        var orgUnit = await Work.GetCollection<OrganizationUnit>(nameof(OrganizationUnit))
            .Find(x => x.OrganizationId == organizationId)
            .ToListAsync();
        if (orgUnit == null)
            throw new EntityNotFoundException("Organization units with organization id: {0} was not found", organizationId.ToString());
        return Mapper.Map<IEnumerable<OrganizationUnitDto>>(orgUnit);
    }

    public async Task<IEnumerable<IdNamePair>> GetOrganizationUnitsShortAsync(int organizationId)
    {
        var orgUnit = await Work.GetCollection<OrganizationUnit>(nameof(OrganizationUnit))
            .Find(x => x.OrganizationId == organizationId)
            .Project(x => new {x.Id, x.Name})
            .ToListAsync();
        if (orgUnit == null)
            throw new EntityNotFoundException("Organization units with organization id: {0} was not found", organizationId.ToString());
        return orgUnit.Select(x => new IdNamePair(x.Id, x.Name));
    }

    public async Task<PositionDto> GetPositionAsync(int organizationId, int organizationUnitId, int id)
    {
        var position = await Work.GetCollection<Position>(nameof(Position))
            .Find(x => x.OrganizationId == organizationId && x.OrganizationUnitId == organizationUnitId && x.Id == id).FirstOrDefaultAsync();
        if (position == null)
            throw new EntityNotFoundException(id.ToString());
        return Mapper.Map<PositionDto>(position);
    }

    public async Task<IEnumerable<PositionDto>> GetPositionsAsync(int organizationId, int? organizationUnitId)
    {
        var positions = await Work.GetCollection<Position>(nameof(Position))
            .Find(x => x.OrganizationId == organizationId && (!organizationUnitId.HasValue || x.OrganizationUnitId == organizationUnitId))
            .ToListAsync();
        if (positions == null)
            throw new EntityNotFoundException("Positions with organization id: {0} was not found", organizationId.ToString());
        return Mapper.Map<IEnumerable<PositionDto>>(positions);
    }

    public async Task<IEnumerable<IdNamePair>> GetPositionsShortAsync(int organizationId, int? organizationUnitId)
    {
        var positions = await Work.GetCollection<Position>(nameof(Position))
            .Find(x => x.OrganizationId == organizationId && (!organizationUnitId.HasValue || x.OrganizationUnitId == organizationUnitId))
            .ToListAsync();
        if (positions == null)
            throw new EntityNotFoundException("Positions with organization id: {0} was not found", organizationId.ToString());
        return positions.Select(x => new IdNamePair(x.Id, x.Name));
    }
}