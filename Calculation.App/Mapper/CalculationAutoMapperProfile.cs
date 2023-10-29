using AutoMapper;
using Calculation.App.Commands;
using Calculation.App.DtoModels;
using Calculation.Data.Entities;

namespace Calculation.App.Mapper;

public class CalculationAutoMapperProfile : Profile
{
    public CalculationAutoMapperProfile()
    {
        CreateMap<Operation, OperationDto>();
        CreateMap<Operation, OperationCreateCommand>();
        CreateMap<Operation, OperationUpdateCommand>();
        CreateMap<PaymentCard, PaymentCardDto>();
        CreateMap<PaymentCard, PaymentCardUpdateCommand>();
        CreateMap<PaymentCardCalculationCommand, CalculationSalaryMessage>();
    }
}