using FilePocket.BlazorClient.Features.Bookmarks.Models;
using FilePocket.BlazorClient.Features.Profiles.Models;
using Newtonsoft.Json;
using System.Text;

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

    public async Task<bool> UpdateAsync(UpdateProfileModel profile)
    {
        var content = GetStringContent(profile);

        var response = await _apiClient.PutAsync(BaseUrl, content);

        return response.IsSuccessStatusCode;
    }

    private static StringContent? GetStringContent(object? obj)
    {
        var json = JsonConvert.SerializeObject(obj);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
