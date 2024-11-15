using Blazored.LocalStorage;
using FilePocket.Client.Features.Authentication;
using FilePocket.Client.Services.Authentication.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FilePocket.Client.Services.Authentication.Requests
{
    public class AuthentictionRequests : IAuthentictionRequests
    {
        private const string HttpClientName = "FilePocketApi";

        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthentictionRequests(
            IHttpClientFactory factory,
            AuthenticationStateProvider authenticationStateProvider,
            ILocalStorageService localStorage)
        {
            _httpClient = factory.CreateClient(HttpClientName);
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<TokenModel> LoginAsync(LoginModel loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync(AuthUrl.LoginUrl, loginModel);
            
            if (!response.IsSuccessStatusCode)
            {
                return new TokenModel();
            }
            
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<TokenModel>(content)!;

            await _localStorage.SetItemAsync("authToken", result.Token);

            ((AuthStateProvider)_authenticationStateProvider).NotifyUserAuthentication(
                loginModel.Email);

            _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("bearer", result.Token);

            return JsonConvert.DeserializeObject<TokenModel>(content)!;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");

            ((AuthStateProvider)_authenticationStateProvider).NotifyUserLogout();

            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<TokenModel> RefreshAccessTokenAsync(TokenModel tokenModel)
        {
            var response = await _httpClient.PostAsJsonAsync(AuthUrl.RefreshTokenUrl, tokenModel);
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TokenModel>(content)!;
        }

        public async Task<bool> RegisterUserAsync(RegistrationRequest registrationRequest)
        {
            var response = await _httpClient.PostAsJsonAsync(AuthUrl.AuthenticationUrl, registrationRequest);

            return response.IsSuccessStatusCode;
        }
    }
}
