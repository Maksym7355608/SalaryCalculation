using AutoMapper;
using Dictionary.App.Commands;
using Dictionary.Models;

namespace Dictionary.App.Mapper;

public class DictionaryAutoMapperProfile : Profile
{
    public DictionaryAutoMapperProfile()
    {
        CreateMap<BaseAmount, BaseAmountCreateCommand>().ReverseMap();
        CreateMap<FinanceData, FinanceDataCreateCommand>().ReverseMap();
        CreateMap<Formula, FormulaCreateCommand>().ReverseMap();
    }
}