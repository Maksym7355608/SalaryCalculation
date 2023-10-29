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
    
    public class AjaxResponse
    {
        public bool IsSuccess { get; set; }
        public string[]? Errors { get; set; }
        public object Data { get; set; }

        public AjaxResponse()
        {
            
        }

        public AjaxResponse(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public AjaxResponse(bool isSuccess, object data): this(isSuccess)
        {
            Data = data;
        }

        public AjaxResponse(bool isSuccess, object data, string[] errors) : this(isSuccess, data)
        {
            Errors = errors;
        }

        public AjaxResponse(bool isSuccess, string[] errors) : this(isSuccess)
        {
            Errors = errors;
        }
    }
}