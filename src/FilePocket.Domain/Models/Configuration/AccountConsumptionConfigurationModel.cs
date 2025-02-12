namespace FilePocket.Domain.Models.Configuration;

public class AccountConsumptionConfigurationModel
{
    public const string Section = "AccountConsumptionSettings";
    
    public StorageConsumptionSettings Storage { get; init; }
}

public class StorageConsumptionSettings
{
    public double CapacityMb { get; init; }
}