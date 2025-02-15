namespace FilePocket.Domain.Entities.Consumption.Errors;

public class AccountConsumptionNotFoundException(Guid userId)
    : InvalidOperationException(AccountConsumptionMessages.StorageConsumptionNotFound(userId));