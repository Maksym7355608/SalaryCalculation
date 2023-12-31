﻿using AutoMapper;
using Organization.App.Commands;
using Organization.App.DtoModels;
using Organization.Data.BaseModels;
using Organization.Data.Entities;
using SalaryCalculation.Data.BaseModels;
using Org = Organization.Data.Entities.Organization;

namespace Organization.App.Mapper;

public class OrganizationAutoMapperProfile : Profile
{
    public OrganizationAutoMapperProfile()
    {
        CreateMap<Org, OrganizationShortDto>().ReverseMap();
        CreateMap<Org, OrganizationDto>()
            .ForMember(x => x.Manager,
                y => y.MapFrom(x =>
                    x.Manager != null
                        ? new IdNamePair(x.Manager.EmployeeId, $"{x.Manager.Name.FirstName} {x.Manager.Name.LastName}")
                        : null))
            .ReverseMap();
        CreateMap<Org, OrganizationUpdateCommand>()
            .ForMember(x => x.Manager,
                y => y.MapFrom(x =>
                    x.Manager != null
                        ? new IdNamePair(x.Manager.EmployeeId, $"{x.Manager.Name.FirstName} {x.Manager.Name.LastName}")
                        : null))
            .ReverseMap();
        CreateMap<Org, OrganizationCreateCommand>()
            .ForMember(x => x.Manager,
                y => y.MapFrom(x =>
                    x.Manager != null
                        ? new IdNamePair(x.Manager.EmployeeId, $"{x.Manager.Name.FirstName} {x.Manager.Name.LastName}")
                        : null))
            .ReverseMap();
        CreateMap<OrganizationPermissionUpdateCommand, OrganizationPermissions>()
            .ForMember(x => x.Permissions,
                y => y.MapFrom(x => x.Permissions.Cast<int>()))
            .ReverseMap();
        CreateMap<Bank, BankDto>().ReverseMap();

        CreateMap<OrganizationUnit, OrganizationUnitDto>().ReverseMap();
        CreateMap<OrganizationUnit, OrganizationUnitUpdateCommand>().ReverseMap();
        CreateMap<OrganizationUnit, OrganizationUnitCreateCommand>().ReverseMap();

        CreateMap<Position, PositionDto>().ReverseMap();
        CreateMap<Position, PositionUpdateCommand>().ReverseMap();
        CreateMap<Position, PositionCreateCommand>().ReverseMap();

        CreateMap<Employee, EmployeeDto>().ReverseMap();
        CreateMap<Employee, EmployeeCreateCommand>().ReverseMap();
        CreateMap<Employee, EmployeeUpdateCommand>().ReverseMap();
        CreateMap<Employee, Manager>()
            .ForMember(x => x.EmployeeId, m => m.MapFrom(y => y.Id))
            .ForMember(x => x.RollNumber, m => m.MapFrom(y => y.RollNumber))
            .ForMember(x => x.OrganizationId, m => m.MapFrom(x => x.Organization.Id))
            .ForMember(x => x.Contacts, m => m.MapFrom(y => y.Contacts))
            .ForMember(x => x.Name, m => m.MapFrom(y => y.Name))
            .ReverseMap();
        CreateMap<Salary, SalaryDto>().ReverseMap();
        CreateMap<Contact, ContactDto>().ReverseMap();
        CreateMap<Person, PersonDto>().ReverseMap();
    }
}