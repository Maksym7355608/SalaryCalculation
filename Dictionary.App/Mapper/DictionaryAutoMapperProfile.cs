using AutoMapper;
using Dictionary.App.Commands;
using Dictionary.App.Dto;
using Dictionary.Models;

namespace Dictionary.App.Mapper;

public class DictionaryAutoMapperProfile : Profile
{
    public DictionaryAutoMapperProfile()
    {
        CreateMap<BaseAmount, BaseAmountCreateCommand>().ReverseMap();
        CreateMap<BaseAmount, BaseAmountDto>()
            .ForMember(dst => dst.Id, src => src.MapFrom(x =>x.Id.ToString()));
        CreateMap<FinanceData, FinanceDataCreateCommand>().ReverseMap();
        CreateMap<FinanceData, FinanceDataDto>()
            .ForMember(dst => dst.Id, src => src.MapFrom(x =>x.Id.ToString()));
        CreateMap<Formula, FormulaCreateCommand>().ReverseMap();
        CreateMap<Formula, FormulaDto>()
            .ForMember(dst => dst.Id, src => src.MapFrom(x =>x.Id.ToString()));
    }
}