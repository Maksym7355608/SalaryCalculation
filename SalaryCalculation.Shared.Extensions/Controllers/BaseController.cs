using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace SalaryCalculation.Shared.Extensions.Controllers;

public abstract class BaseController : ControllerBase
{
    protected readonly IMapper Mapper;
    protected bool IsValid => ModelState.IsValid;
    protected string[] Errors => ModelState.Values.SelectMany(x => x.Errors.Select(x => x.ErrorMessage)).ToArray();

    public BaseController(IMapper mapper)
    {
        Mapper = mapper;
    }
    
    public IActionResult RestAjaxResponse(bool success, string[]? errors = null, object data = null)
    {
        if(errors != null && errors.Any())
            return BadRequest(new {success = success, errors = errors, data = data});
        else
            return Ok(new {success = success, errors = errors, data = data});
    }
}