using AutoMapper;
using Calculation.App.Abstract;
using Calculation.App.Commands;
using Calculation.App.DtoModels;
using Calculation.Data;
using Calculation.Data.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SalaryCalculation.Data.BaseHandlers;
using SalaryCalculation.Shared.Common.Validation;
using SalaryCalculation.Shared.Extensions.PeriodExtensions;

namespace Calculation.App.Handlers;

public class PaymentCardCommandHandler : BaseCalculationCommandHandler, IPaymentCardCommandHandler
{
    private IOperationCommandHandler _operationCommandHandler;
    
    public PaymentCardCommandHandler(ICalculationUnitOfWork work, ILogger<BaseCommandHandler> logger, IMapper mapper,
        IOperationCommandHandler operationCommandHandler) : base(work, logger, mapper)
    {
        _operationCommandHandler = operationCommandHandler;
    }

    public async Task<PaymentCardDto> GetPaymentCardAsync(int id)
    {
        var card = await Work.GetCollection<PaymentCard>()
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
        if (card == null)
            throw new EntityNotFoundException(id.ToString());
        return Mapper.Map<PaymentCardDto>(card);
    }

    public async Task<IEnumerable<PaymentCardDto>> GetPaymentCardsByEmployeeAsync(int employeeId, int? period)
    {
        var cards = await Work.GetCollection<PaymentCard>()
            .Find(x => x.Employee.Id == employeeId &&
                       (!period.HasValue || x.CalculationPeriod == period.Value))
            .ToListAsync();
        if (cards == null || cards.Count == 0)
            throw new EntityNotFoundException("Payment cards with employee id: {0} " + (period.HasValue ? "and period {1} " : string.Empty) +
                                              "was not found", employeeId.ToString(), period?.ToShortPeriodString() ?? string.Empty);
        return Mapper.Map<IEnumerable<PaymentCardDto>>(cards);
    }

    public async Task<IEnumerable<PaymentCardDto>> SearchPaymentCardsAsync(PaymentCardSearchCommand command)
    {
        var filterBuilder = Builders<PaymentCard>.Filter;
        var filter = filterBuilder.Empty;
        if (command.Id.HasValue)
        {
            filter &= filterBuilder.Eq(p => p.Id, command.Id.Value);
        }
        if (command.CalculationPeriod.HasValue)
        {
            filter &= filterBuilder.Eq(p => p.CalculationPeriod, command.CalculationPeriod.Value);
        }
        if (command.OrganizationId > 0)
        {
            filter &= filterBuilder.Eq(p => p.OrganizationId, command.OrganizationId);
        }
        if (command.EmployeeNumbers != null && command.EmployeeNumbers.Any())
        {
            filter &= filterBuilder.In(p => p.Employee.Name, command.EmployeeNumbers);
        }
        var paymentCards = await Work.GetCollection<PaymentCard>().Find(filter).ToListAsync();
        
        var paymentCardDtos = Mapper.Map<IEnumerable<PaymentCardDto>>(paymentCards);

        return paymentCardDtos;
    }

    public async Task CalculatePaymentCardAsync(PaymentCardCalculationCommand command)
    {
        //TODO: release logic
        throw new NotImplementedException();
    }

    public async Task<bool> UpdatePaymentCardsAsync(PaymentCardUpdateCommand command)
    {
        
        var paymentCard = Mapper.Map<PaymentCard>(command);

        var filter = Builders<PaymentCard>.Filter.Eq(pc => pc.Id, command.Id);
        var existingPaymentCard = await Work.GetCollection<PaymentCard>().Find(filter).SingleOrDefaultAsync();

        if (existingPaymentCard == null)
            throw new EntityNotFoundException("Payment card with id: {0} was not found", command.Id.ToString());

        existingPaymentCard.PaymentDate = paymentCard.PaymentDate;
        existingPaymentCard.CalculationPeriod = paymentCard.CalculationPeriod;
        existingPaymentCard.PayedAmount = paymentCard.PayedAmount;
        existingPaymentCard.AccrualAmount = paymentCard.AccrualAmount;
        existingPaymentCard.MaintenanceAmount = paymentCard.MaintenanceAmount;
        var results = new List<Task<bool>>();
        foreach (var opCmd in command.OperationsCommand)
            results.Add(_operationCommandHandler.UpdateOperationAsync(opCmd));
        await Task.WhenAll(results);
        
        await Work.GetCollection<PaymentCard>().ReplaceOneAsync(filter, existingPaymentCard);

        return true;
    }

    public async Task<bool> DeletePaymentCardAsync(int id)
    {
        var result = await Work.GetCollection<PaymentCard>().DeleteOneAsync(x => x.Id == id);
        if(result.DeletedCount == 0)
            throw new InvalidOperationException("Payment card was no delete");
        return true;
    }
}