using AutoMapper;
using Dictionary.App.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalaryCalculation.Shared.Common.Attributes;
using SalaryCalculation.Shared.Common.Controllers;

namespace Dictionary.API.Controllers;

[ApiController]
[ServiceFilter(typeof(HandleExceptionAttribute))]
[Authorize]
[Route("api/[controller]")]
public class BaseDictionaryController : BaseController
{
    protected readonly IDictionaryCommandHandler DictionaryCommandHandler;
    
    public BaseDictionaryController(IDictionaryCommandHandler dictionaryCommandHandler, IMapper mapper) : base(mapper)
    {
        DictionaryCommandHandler = dictionaryCommandHandler;
    }
}