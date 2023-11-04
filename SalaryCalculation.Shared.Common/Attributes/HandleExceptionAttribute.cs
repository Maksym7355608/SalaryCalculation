using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SalaryCalculation.Shared.Common.Controllers;
using SalaryCalculation.Shared.Common.Validation;

namespace SalaryCalculation.Shared.Common.Attributes;

public class HandleExceptionAttribute : Attribute, IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Ловимо винятки та обробляємо їх
        if (context.Exception != null)
        {
            // Отримуємо повідомлення про помилку
            var errorMessage = context.Exception.Message;
            
            if (context.Exception is EntityNotFoundException customException)
            {
                context.ModelState.AddModelError("", customException.Message);
            }
            else
            {
                context.ModelState.AddModelError("", errorMessage);
            }
            
            context.Result = new JsonResult(new BaseController.AjaxResponse
            {
                IsSuccess = context.ModelState.IsValid,
                Errors = context.ModelState.Values.SelectMany(x => x.Errors.Select(x => x.ErrorMessage)).ToArray()
            });
            context.ExceptionHandled = true;
        }
    }
}