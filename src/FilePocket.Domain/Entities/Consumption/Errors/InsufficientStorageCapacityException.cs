using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Entities.Consumption.Errors;

public class InsufficientStorageCapacityException(double remainingMb, double additionalUsedMb) 
    : ValidationException(AccountConsumptionMessages.InsufficientStorageCapacity(remainingMb, additionalUsedMb));