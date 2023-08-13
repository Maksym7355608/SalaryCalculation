using AutoMapper;
using Organization.App.Commands;
using Organization.App.DtoModels;
using Organization.Data.Entities;
using Org = Organization.Data.Entities.Organization;

namespace Organization.App.Mapper;

public class OrganizationAutoMapperProfile : Profile
{
    public OrganizationAutoMapperProfile()
    {
        CreateMap<Org, OrganizationDto>();
        CreateMap<Org, OrganizationUpdateCommand>();
        CreateMap<OrganizationPermissionUpdateCommand, OrganizationPermissions>()
            .ForMember(x => x.Permissions, 
                y => y.MapFrom(x => x.Permissions.Cast<int>()));

        CreateMap<OrganizationUnit, OrganizationUnitDto>();
        CreateMap<OrganizationUnit, OrganizationUnitUpdateCommand>();

        CreateMap<Position, PositionDto>();
        CreateMap<Position, PositionUpdateCommand>();

        CreateMap<Employee, EmployeeDto>();
        CreateMap<Employee, EmployeeCreateCommand>();
        CreateMap<Employee, EmployeeUpdateCommand>();
        CreateMap<Employee, Manager>()
            .ForMember(x => x.EmployeeId, m => m.MapFrom(y => y.Id))
            .ForMember(x => x.RollNumber, m => m.MapFrom(y => y.RollNumber))
            .ForMember(x => x.OrganizationId, m => m.MapFrom(x => x.Organization.Id))
            .ForMember(x => x.Contacts, m => m.MapFrom(y => y.Contacts))
            .ForMember(x => x.Name, m => m.MapFrom(y => y.Name))
            .ReverseMap();
    }
}