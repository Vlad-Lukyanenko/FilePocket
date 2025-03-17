using FilePocket.BlazorClient.Features.Profiles.Models;
using Newtonsoft.Json;

namespace FilePocket.BlazorClient.Features.Profiles.Requests;

public class ProfileRequests : IProfileRequests
{
    private readonly FilePocketApiClient _apiClient;
    private const string BaseUrl = "api/profile";

    public ProfileRequests(FilePocketApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ProfileModel> GetByUserIdAsync(Guid userId)
    {
        var content = await _apiClient.GetAsync($"{BaseUrl}/userId/{userId}");

        return JsonConvert.DeserializeObject<ProfileModel>(content)!;
    }
}
