using FilePocket.Domain.Entities.Consumption;
using FilePocket.Domain.Models.Configuration;
using Microsoft.AspNetCore.Identity;

namespace FilePocket.Domain.Entities;

// TODO: Split User into PocketAccount to separate user data from account data.
// User should only contain identity specific data. Account should handle application specific data.
public class User : IdentityUser<Guid>
{
    // Identity specific data
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    // Account specific data
    public virtual Profile Profile { get; set; } = null!;
    public virtual ICollection<Pocket> Pockets { get; private set; } = [];
    public virtual ICollection<FileMetadata> FilesMetadata { get; private set; } = [];// will be gone after default pocket is implemented
    public virtual ICollection<AccountConsumption> AccountConsumptions { get; private set; } = [];

    public User WithId()
    {
        Id = Guid.NewGuid();
        return this;
    }
    
    public User WithUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username must not be empty.");
        
        UserName = username;

        return this;
    }

    public User WithFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name must not be empty.");
        
        FirstName = firstName;

        return this;
    }
    
    public User WithLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name must not be empty.");
        
        LastName = lastName;

        return this;
    }

    public User WithRefreshToken(string refreshToken, DateTime refreshTokenExpiryTime)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token must not be empty.");
        
        if (refreshTokenExpiryTime <= DateTime.UtcNow)
            throw new ArgumentException("Refresh token expiry time must be in the future.");

        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = refreshTokenExpiryTime;

        return this;
    }

    public User WithPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password must not be empty.");
        
        PasswordHash = new PasswordHasher<User>().HashPassword(this, password);
        return this;
    }

    public void ConfigureAccountConsumptions(AccountConsumptionConfigurationModel? accountConsumptionConfiguration = null)
    {
        // not sure how the update will look like, so configuration model is ok for now
        var storageConsumption = accountConsumptionConfiguration is not null 
            ? StorageConsumption.Activate(Id, accountConsumptionConfiguration.Storage.CapacityMb)
            : StorageConsumption.Activate(Id);

        ConfigureStorageConsumption(storageConsumption);
    }

    private void ConfigureStorageConsumption(StorageConsumption storageConsumption)
    {
        if (IsStorageConsumptionConfigured())
            return;

        AccountConsumptions.Add(storageConsumption);
    }

    private bool IsStorageConsumptionConfigured()
        => AccountConsumptions.Any(x => x.MetricType == AccountConsumption.StorageCapacity);
}
