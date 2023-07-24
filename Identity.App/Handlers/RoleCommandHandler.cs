using AutoMapper;
using Identity.App.Abstract;
using Identity.App.Commands;
using Identity.App.DtoModels;
using Identity.Data.Data;
using Identity.Data.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using SalaryCalculation.Shared.Common.Validation;

namespace Identity.App.Handlers;

public class RoleCommandHandler : BaseIdentityCommandHandler, IRoleCommandHandler
{
    public RoleCommandHandler(IIdentityUnitOfWork work, IMapper mapper) : base(work, mapper)
    { }
    
    public async Task CreateRole(RoleCreateCommand command)
    {
        var role = Mapper.Map<Role>(command);
        await Work.GetCollection<Role>()
            .InsertOneAsync(role);
    }

    public async Task<bool> UpdateRole(ObjectId roleId, RoleUpdateCommand command)
    {
        var role = Mapper.Map<Role>(command);
        var result = await Work.GetCollection<Role>(nameof(Role))
            .ReplaceOneAsync(x => x.Id == roleId, role);

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteRole(ObjectId roleId)
    {
        var result = await Work.GetCollection<Role>(nameof(Role))
            .DeleteOneAsync(x => x.Id == roleId);

        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public async Task<IEnumerable<RoleDto>> SearchRoles(RoleSearchCommand command)
    {
        var roles = await Work.GetCollection<Role>(nameof(Role))
            .Find(GetRoleFilter(command))
            .ToListAsync();
        return Mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    private FilterDefinition<Role> GetRoleFilter(RoleSearchCommand command)
    {
        var filterBuilder = Builders<Role>.Filter;

        var filter = new List<FilterDefinition<Role>>()
        {
            filterBuilder.Eq(x => x.OrganizationId, command.OrganizationId)
        };
        if (command.Ids.Length > 0)
            filter.Add(filterBuilder.In(x => x.Id, command.Ids));
        if (string.IsNullOrWhiteSpace(command.SearchName))
            filter.Add(filterBuilder.Where(x => x.NormalizedName.Contains(command.SearchName.Normalize())));
        return filterBuilder.And(filter);
    }

    public async Task<RoleDto> GetRoleById(ObjectId id)
    {
        var role = await Work.GetCollection<Role>(nameof(Role))
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
        if (role == null)
            throw new EntityNotFoundException("Role with id {0} was not found", id.ToString());
        
        return Mapper.Map<RoleDto>(role);
    }
}