namespace FilePocket.Domain.Entities.Consumption.Errors;

public static class AccountConsumptionMessages
{
    public static string InsufficientStorageCapacity(double remainingMb, double additionalUsedMb) 
        => $"Account consumptions: Insufficient storage capacity to handle file upload with size {additionalUsedMb}. Remaining: {remainingMb}.";

    public static string UsedAmountMustBePositive => "Used amount must be positive.";
    public static string FreeAmountMustBePositive => "Free amount must be positive.";
    public static string AmountMustBePositive => "Amount must be positive.";
    public static string UserIdMustBeSpecified => "User ID must be specified to configure account consumption.";
    public static string StorageCapacityUsed(double used, double total) => $"Storage capacity used: {used} / {total}";
    public static string StorageConsumptionNotFound(Guid userId) => $"Account storage consumption record not found for user ID: {userId}.";
}