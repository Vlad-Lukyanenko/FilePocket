using FilePocket.Admin.Models.Authentication;
using FilePocket.Admin.Requests.Contracts;
using Newtonsoft.Json;

namespace FilePocket.Admin.Requests;

public class AuthentictionRequests : IAuthentictionRequests
{
    private readonly HttpClient _httpClient;

    public AuthentictionRequests(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("AuthApi");
    }

    public async Task<TokenModel> LoginUserAsync(LoginModel loginModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/authentication/login", loginModel);
        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<TokenModel>(content)!;
    }

    public async Task<TokenModel> RefreshAccessTokenAsync(TokenModel tokenModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/token/refresh", tokenModel);
        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<TokenModel>(content)!;
    }

    public async Task<bool> RegisterUserAsync(RegistrationModel registrationModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/authentication", registrationModel);

        return response.IsSuccessStatusCode;
    }
}
