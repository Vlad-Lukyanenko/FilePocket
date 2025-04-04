using FilePocket.BlazorClient.Features.Users.Models;
using Newtonsoft.Json;
using System.Text;

namespace FilePocket.BlazorClient.Features.Users.Requests;

public class UserRequests : IUserRequests
{
    private readonly FilePocketApiClient _apiClient;

    public UserRequests(FilePocketApiClient authorizedRequests)
        => _apiClient = authorizedRequests;

    public async Task<LoggedInUserModel?> GetByUserNameAsync(string userName)
    {
        var response = await _apiClient.GetAsync($"api/users/{userName}");

        return JsonConvert.DeserializeObject<LoggedInUserModel>(response);
    }

    public Task UpdateUserAsync(UpdateUserRequest updateUserRequest)
    {
        var json = JsonConvert.SerializeObject(updateUserRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return _apiClient.PutAsync($"api/users", content);
    }
}
