namespace FilePocket.Domain.Entities.Consumption.Errors;

public static class AccountConsumptionMessages
{
    public static string InsufficientStorageCapacity(double used, double total, double current) 
        => $"Insufficient storage capacity to upload file. Used/Total: {used.FormatFileSize()} / {total.FormatFileSize()}. Current file size: {current.FormatFileSize()}.";

    public static string UsedAmountMustBePositive => "Used amount must be positive.";
    public static string FreeAmountMustBePositive => "Free amount must be positive.";
    public static string AmountMustBePositive => "Amount must be positive.";
    public static string UserIdMustBeSpecified => "User ID must be specified to configure account consumption.";
    public static string StorageCapacityUsed(double used, double total) => $"Storage capacity used: {used.FormatFileSize()} / {total.FormatFileSize()}";
    public static string StorageConsumptionNotFound(Guid userId) => $"Account storage consumption record not found for user ID: {userId}.";
}

internal static class FileSizeFormatingExtensions
{
    public static string FormatFileSize(this double sizeInMb)
    {
        // Define conversion factors.
        const double kbPerMb = 1024;
        const double mbThreshold = 0.1; // If below 0.1 MB, show in KB.
    
        if (sizeInMb < mbThreshold)
        {
            var sizeInKb = sizeInMb * kbPerMb;
            return $"{sizeInKb:0.##} KB";
        }
    
        return $"{sizeInMb:0.##} MB";
    }
}