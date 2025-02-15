using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Entities.Consumption.Errors;

public class InsufficientStorageCapacityException(double usedMb, double totalMb, double additionalUsedMb) 
    : ValidationException(AccountConsumptionMessages.InsufficientStorageCapacity(usedMb, totalMb, additionalUsedMb));