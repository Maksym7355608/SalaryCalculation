using Dictionary.App.Commands;
using Dictionary.App.Dto;
using Dictionary.Models;
using MongoDB.Bson;

namespace Dictionary.App.Abstract;

public interface IDictionaryCommandHandler
{
    Task<List<BaseAmountDto>> SearchBaseAmounts(BaseAmountsSearchCommand command);
    Task<List<FinanceDataDto>> SearchFinanceData(FinanceDataSearchCommand command);
    Task<List<FormulaDto>> SearchFormulas(FormulasSearchCommand command);

    Task<bool> CreateBaseAmount(BaseAmountCreateCommand command);
    Task<bool> CreateFinanceData(FinanceDataCreateCommand command);
    Task<bool> CreateFormula(FormulaCreateCommand command);

    Task<bool> UpdateBaseAmount(ObjectId id, BaseAmountCreateCommand command);
    Task<bool> UpdateFinanceData(ObjectId id, FinanceDataCreateCommand command);
    Task<bool> UpdateFormula(ObjectId id, FormulaCreateCommand command);

    Task<bool> DeleteBaseAmount(ObjectId id);
    Task<bool> DeleteFinanceData(ObjectId id);
    Task<bool> DeleteFormula(ObjectId id);
}