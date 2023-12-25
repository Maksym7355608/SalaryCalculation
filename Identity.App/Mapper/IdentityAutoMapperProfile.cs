using AutoMapper;
using Identity.App.Commands;
using Identity.App.DtoModels;
using Identity.Data.Entities;
using MongoDB.Bson;

namespace Identity.App.Mapper;

public class IdentityAutoMapperProfile : Profile
{
    public IdentityAutoMapperProfile()
    {
        CreateMap<User, UserCreateCommand>().ReverseMap();
        CreateMap<User, UserUpdateCommand>()
            .ForMember(d => d.Roles, 
                s => s.MapFrom(x => 
                    x.Roles.Select(r => r.ToString())));
        CreateMap<UserUpdateCommand, User>()
            .ForMember(d => d.Roles, 
                s => s.MapFrom(x => 
                    x.Roles.Select(r => ObjectId.Parse(r))));
        CreateMap<Role, RoleDto>().ReverseMap();
        CreateMap<Role, RoleCreateCommand>().ReverseMap();
        CreateMap<Role, RoleUpdateCommand>().ReverseMap();
    }
}