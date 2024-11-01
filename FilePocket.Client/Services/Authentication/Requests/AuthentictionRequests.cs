using FilePocket.Client.Services.Authentication.Models;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace FilePocket.Client.Services.Authentication.Requests
{
    public class AuthentictionRequests : IAuthentictionRequests
    {
        private const string HttpClientName = "FilePocketApi";

        private readonly HttpClient _httpClient;

        public AuthentictionRequests(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient(HttpClientName);
        }

        public async Task<TokenModel> LoginUserAsync(LoginModel loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync(AuthUrl.LoginUrl, loginModel);
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TokenModel>(content)!;
        }

        public async Task<TokenModel> RefreshAccessTokenAsync(TokenModel tokenModel)
        {
            var response = await _httpClient.PostAsJsonAsync(AuthUrl.RefreshTokenUrl, tokenModel);
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TokenModel>(content)!;
        }

        public async Task<bool> RegisterUserAsync(RegistrationModel registrationModel)
        {
            var response = await _httpClient.PostAsJsonAsync(AuthUrl.AuthenticationUrl, registrationModel);

            return response.IsSuccessStatusCode;
        }
    }
}
