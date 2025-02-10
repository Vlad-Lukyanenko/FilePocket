namespace FilePocket.Domain.Entities.Consumption.Errors;

public class UserIdMustBeSpecifiedException() : ArgumentException(AccountConsumptionMessages.UserIdMustBeSpecified);