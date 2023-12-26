using AutoMapper;
using Dictionary.App.Abstract;
using Dictionary.App.Commands;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using SalaryCalculation.Shared.Common.Attributes;

namespace Dictionary.API.Controllers;

[ApiController]
[HandleException]
[Route("api/[controller]")]
public class DictionaryController : BaseDictionaryController
{
    public DictionaryController(IDictionaryCommandHandler dictionaryCommandHandler, IMapper mapper) : base(dictionaryCommandHandler, mapper)
    {
    }

    #region Base Amounts

    [HttpPost("base-amount/search")]
    public async Task<IActionResult> SearchBaseAmountAsync([FromBody] BaseAmountsSearchCommand command)
    {
        var result = await DictionaryCommandHandler.SearchBaseAmounts(command);
        return GetAjaxResponse(IsValid, result, Errors);
    }

    [HttpPost("base-amount/create")]
    public async Task<IActionResult> CreateBaseAmountAsync([FromBody] BaseAmountCreateCommand command)
    {
        var result = await DictionaryCommandHandler.CreateBaseAmount(command);
        return GetAjaxResponse(IsValid, result, Errors);
    }

    [HttpPut("base-amount/update/{id}")]
    public async Task<IActionResult> UpdateBaseAmountAsync([FromRoute] ObjectId id, [FromBody] BaseAmountCreateCommand command)
    {
        var result = await DictionaryCommandHandler.UpdateBaseAmount(id, command);
        return GetAjaxResponse(IsValid, result, Errors);
    }

    [HttpDelete("base-amount/delete/{id}")]
    public async Task<IActionResult> CreateBaseAmountAsync([FromRoute] ObjectId id)
    {
        var result = await DictionaryCommandHandler.DeleteBaseAmount(id);
        return GetAjaxResponse(IsValid, result, Errors);
    }
    
    #endregion

    #region Finance Data

    [HttpPost("finance-data/search")]
    public async Task<IActionResult> SearchFinanceDataAsync([FromBody] FinanceDataSearchCommand command)
    {
        var result = await DictionaryCommandHandler.SearchFinanceData(command);
        return GetAjaxResponse(IsValid, result, Errors);
    }

    [HttpPost("finance-data/create")]
    public async Task<IActionResult> CreateFinanceDataAsync([FromBody] FinanceDataCreateCommand command)
    {
        var result = await DictionaryCommandHandler.CreateFinanceData(command);
        return GetAjaxResponse(IsValid, result, Errors);
    }

    [HttpPut("finance-data/update/{id}")]
    public async Task<IActionResult> UpdateFinanceDataAsync([FromRoute] ObjectId id, [FromBody] FinanceDataCreateCommand command)
    {
        var result = await DictionaryCommandHandler.UpdateFinanceData(id, command);
        return GetAjaxResponse(IsValid, result, Errors);
    }

    [HttpDelete("finance-data/delete/{id}")]
    public async Task<IActionResult> CreateFinanceDataAsync([FromRoute] ObjectId id)
    {
        var result = await DictionaryCommandHandler.DeleteFinanceData(id);
        return GetAjaxResponse(IsValid, result, Errors);
    }
    
    #endregion
    
    #region Formula

    [HttpPost("formula/search")]
    public async Task<IActionResult> SearchFormulaAsync([FromBody] FormulasSearchCommand command)
    {
        var result = await DictionaryCommandHandler.SearchFormulas(command);
        return GetAjaxResponse(IsValid, result, Errors);
    }

    [HttpPost("formula/create")]
    public async Task<IActionResult> CreateFormulaAsync([FromBody] FormulaCreateCommand command)
    {
        var result = await DictionaryCommandHandler.CreateFormula(command);
        return GetAjaxResponse(IsValid, result, Errors);
    }

    [HttpPut("formula/update/{id}")]
    public async Task<IActionResult> UpdateFormulaAsync([FromRoute] ObjectId id, [FromBody] FormulaCreateCommand command)
    {
        var result = await DictionaryCommandHandler.UpdateFormula(id, command);
        return GetAjaxResponse(IsValid, result, Errors);
    }

    [HttpDelete("formula/delete/{id}")]
    public async Task<IActionResult> CreateFormulaAsync([FromRoute] ObjectId id)
    {
        var result = await DictionaryCommandHandler.DeleteFormula(id);
        return GetAjaxResponse(IsValid, result, Errors);
    }
    
    #endregion
    
}