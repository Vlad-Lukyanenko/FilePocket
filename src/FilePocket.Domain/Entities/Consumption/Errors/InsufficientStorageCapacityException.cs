namespace FilePocket.Domain.Entities.Consumption.Errors;

public class InsufficientStorageCapacityException(double remainingMb, double additionalUsedMb) 
    : InvalidOperationException(AccountConsumptionMessages.InsufficientStorageCapacity(remainingMb, additionalUsedMb));