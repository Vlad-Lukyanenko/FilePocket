namespace FilePocket.Domain.Entities.Consumption.Errors;

public class UsedAmountMustBePositiveException() : ArgumentException(AccountConsumptionMessages.AmountMustBePositive);