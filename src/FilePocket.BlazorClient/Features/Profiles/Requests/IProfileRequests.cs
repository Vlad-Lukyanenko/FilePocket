using FilePocket.BlazorClient.Features.Profiles.Models;

namespace FilePocket.BlazorClient.Features.Profiles.Requests;

public interface IProfileRequests
{
    Task<ProfileModel> GetByUserIdAsync(Guid userId);
    Task<bool> UpdateAsync(UpdateProfileModel profile);
}
