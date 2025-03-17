using FilePocket.Domain.Entities;

namespace FilePocket.Application.Interfaces.Repositories;

public interface IProfileRepository
{
    Task<Profile> GetByIdAsync(Guid id);
    Task<Profile> GetByUserIdAsync(Guid userId);
    void CreateProfile(Profile profile);
}
