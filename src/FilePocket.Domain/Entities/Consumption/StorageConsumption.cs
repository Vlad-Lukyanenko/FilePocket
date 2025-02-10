using FilePocket.Domain.Entities.Consumption.Errors;

namespace FilePocket.Domain.Entities.Consumption;

public class StorageConsumption : AccountConsumption
{
    private const double TotalSizeInMbsByDefault = 1_000;
    public double Used { get; private set; }
    public double Total { get; private set; }
    public double RemainingSizeMb => Total - Used;
    public override string MetricType { get; protected set; } = StorageCapacity;

    /// <summary>
    /// For debugging/logging purposes.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => AccountConsumptionMessages.StorageCapacityUsed(Used, Total);

    /// <summary>
    /// When a user uploads a file, we increase the space used by the file.
    /// </summary>
    /// <param name="additionalUsedMb"></param>
    /// <exception cref="UsedAmountMustBePositiveException"></exception>
    /// <exception cref="InsufficientStorageCapacityException"></exception>
    public void IncreaseUsage(double additionalUsedMb)
    {
        if (additionalUsedMb < 0)
            throw new UsedAmountMustBePositiveException();

        if (RemainingSizeMb < additionalUsedMb)
            throw new InsufficientStorageCapacityException(RemainingSizeMb, additionalUsedMb);

        Used += additionalUsedMb;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// When a user deletes a file, we free up the space used by the file.
    /// </summary>
    /// <param name="amountToFree"></param>
    /// <exception cref="FreeAmountMustBePositiveException"></exception>
    public void DecreaseUsage(double amountToFree)
    {
        if (amountToFree <= 0)
            throw new FreeAmountMustBePositiveException();

        var newUsed = Used - amountToFree;
        Used = newUsed >= 0 ? newUsed : 0;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// To encourage users to upgrade their storage capacity, we allow them to increase their total size.
    /// </summary>
    /// <param name="newTotalSize"></param>
    /// <exception cref="TotalAmountMustBePositiveException"></exception>
    public void UpdateTotalSize(double newTotalSize)
    {
        if (newTotalSize <= 0)
            throw new TotalAmountMustBePositiveException();

        Total = newTotalSize;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// When consumption is deactivated, the storage capacity will not be honored when a user uploads a file.
    /// </summary>
    public void Deactivate()
    {
        IsActivated = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activate storage capacity for a user with default total size of 1,000 MBs.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="totalSizeInMbs"></param>
    /// <returns></returns>
    /// <exception cref="UserIdMustBeSpecifiedException"></exception>
    /// <exception cref="TotalAmountMustBePositiveException"></exception>
    public static StorageConsumption Activate(in Guid userId, in double? totalSizeInMbs = null)
    {
        if (userId == Guid.Empty)
            throw new UserIdMustBeSpecifiedException();

        if (totalSizeInMbs <= 0)
            throw new TotalAmountMustBePositiveException();

        var total = totalSizeInMbs.GetValueOrDefault() <= TotalSizeInMbsByDefault 
            ? totalSizeInMbs.GetValueOrDefault() 
            : TotalSizeInMbsByDefault;

        var consumption = new StorageConsumption
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Used = 0,
            Total = total,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActivated = true
        };

        return consumption;
    }
}