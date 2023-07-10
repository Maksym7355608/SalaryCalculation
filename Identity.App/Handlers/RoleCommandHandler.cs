using AutoMapper;
using Identity.App.Abstract;
using Identity.App.Commands;
using Identity.Data.Data;
using Identity.Data.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

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
}