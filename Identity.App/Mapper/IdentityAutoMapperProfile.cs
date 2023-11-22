using AutoMapper;
using Identity.App.Commands;
using Identity.App.DtoModels;
using Identity.Data.Entities;

namespace Identity.App.Mapper;

public class IdentityAutoMapperProfile : Profile
{
    public IdentityAutoMapperProfile()
    {
        CreateMap<User, UserCreateCommand>().ReverseMap();
        CreateMap<User, UserUpdateCommand>().ReverseMap();
        CreateMap<Role, RoleDto>().ReverseMap();
        CreateMap<Role, RoleCreateCommand>().ReverseMap();
        CreateMap<Role, RoleUpdateCommand>().ReverseMap();
    }
}