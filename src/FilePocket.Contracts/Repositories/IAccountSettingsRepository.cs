using FilePocket.Domain.Entities;

namespace FilePocket.Contracts.Repositories
{
    public interface IAccountSettingsRepository
    {
        void AddSettings(AccountSettings accountSettings);
        void UpdateSettings(AccountSettings accountSettings);
    }
}
