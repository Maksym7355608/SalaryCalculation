using AutoMapper;
using Dictionary.App.Abstract;
using SalaryCalculation.Shared.Common.Controllers;

namespace Dictionary.API.Controllers;

public class BaseDictionaryController : BaseController
{
    protected readonly IDictionaryCommandHandler DictionaryCommandHandler;
    
    public BaseDictionaryController(IDictionaryCommandHandler dictionaryCommandHandler, IMapper mapper) : base(mapper)
    {
        DictionaryCommandHandler = dictionaryCommandHandler;
    }
}