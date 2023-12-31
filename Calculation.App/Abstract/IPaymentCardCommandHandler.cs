﻿using Calculation.App.Commands;
using Calculation.App.DtoModels;

namespace Calculation.App.Abstract;

public interface IPaymentCardCommandHandler
{
    Task<PaymentCardDto> GetPaymentCardAsync(int id);
    Task<IEnumerable<PaymentCardDto>> GetPaymentCardsByEmployeeAsync(int employeeId, int? period);
    Task<IEnumerable<PaymentCardDto>> SearchPaymentCardsAsync(PaymentCardSearchCommand command);
    Task<string> CalculatePaymentCardAsync(PaymentCardCalculationCommand command);
    Task<bool> UpdatePaymentCardsAsync(PaymentCardUpdateCommand command);
    Task<bool> DeletePaymentCardAsync(int id);
    Task<string> MassCalculatePaymentCardAsync(MassCalculationMessage message);
}