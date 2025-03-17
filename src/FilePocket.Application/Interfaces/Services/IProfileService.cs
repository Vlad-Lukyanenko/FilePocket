using FilePocket.Domain.Models;

namespace FilePocket.Application.Interfaces.Services;

public interface IProfileService
{
    Task<ProfileModel> GetByIdAsync(Guid id);
    Task<ProfileModel> GetByUserIdAsync(Guid userId);
    Task<ProfileModel> CreateProfileAsync(ProfileModel profile);
}
