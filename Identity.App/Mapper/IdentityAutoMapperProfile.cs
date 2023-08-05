using AutoMapper;
using Identity.App.Commands;
using Identity.App.DtoModels;
using Identity.Data.Entities;

namespace Identity.App.Mapper;

public class IdentityAutoMapperProfile : Profile
{
    public IdentityAutoMapperProfile()
    {
        CreateMap<User, UserUpdateCommand>();
        CreateMap<Role, RoleDto>();
    }
}