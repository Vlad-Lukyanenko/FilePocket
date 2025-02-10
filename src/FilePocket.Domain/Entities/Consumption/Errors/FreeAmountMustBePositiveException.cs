namespace FilePocket.Domain.Entities.Consumption.Errors;

public class FreeAmountMustBePositiveException() : ArgumentException(AccountConsumptionMessages.FreeAmountMustBePositive);