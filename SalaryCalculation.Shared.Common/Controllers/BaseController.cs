using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace SalaryCalculation.Shared.Common.Controllers;

public abstract class BaseController : ControllerBase
{
    protected string BearerToken { get; set; }
    protected readonly IMapper Mapper;
    protected bool IsValid => ModelState.IsValid;
    protected string[] Errors => ModelState.Values.SelectMany(x => x.Errors.Select(x => x.ErrorMessage)).ToArray();

    public BaseController(IMapper mapper)
    {
        Mapper = mapper;
    }
    
    public JsonResult RestAjaxResponse(bool success, string[]? errors = null, object data = null)
    {
        return new JsonResult(new {success = success, errors = errors, data = data});
    }
}