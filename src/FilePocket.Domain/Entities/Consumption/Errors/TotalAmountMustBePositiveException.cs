namespace FilePocket.Domain.Entities.Consumption.Errors;

public class TotalAmountMustBePositiveException() : ArgumentException(AccountConsumptionMessages.AmountMustBePositive);