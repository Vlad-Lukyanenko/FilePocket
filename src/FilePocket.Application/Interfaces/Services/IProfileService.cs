using FilePocket.Domain.Models;

namespace FilePocket.Application.Interfaces.Services;

public interface IProfileService
{
    Task<ProfileModel> CreateProfileAsync(ProfileModel profile);
}
