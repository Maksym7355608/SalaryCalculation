using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SalaryCalculation.Shared.Common.Attributes;

public class HandleExceptionAttribute : Attribute
{
    public void OnException(ExceptionContext context)
    {
        // Ловимо винятки та обробляємо їх
        if (context.Exception != null)
        {
            // Отримуємо повідомлення про помилку
            string errorMessage = context.Exception.Message;

            // Якщо виняток - ваш власний тип, можна використати його дані для обробки
            // наприклад, можна отримати додаткові відомості про помилку, логувати її і т.д.
            // if (context.Exception is YourCustomException customException)
            // {
            //     errorMessage = customException.Message;
            //     // Інші дії з помилкою за потреби
            // }

            // Відповідь з кодом помилки 500 та повідомленням про помилку
            context.Result = new ObjectResult(new { error = errorMessage }) { StatusCode = 500 };
            context.ExceptionHandled = true;
        }
    }
}