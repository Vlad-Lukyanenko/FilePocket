using Blazored.LocalStorage;
using FilePocket.Client.Features;
using FilePocket.Client.Features.Authentication;
using FilePocket.Client.Services.Authentication.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FilePocket.Client.Services.Authentication.Requests
{
    public class AuthentictionRequests : IAuthentictionRequests
    {
        private const string HttpClientName = "FilePocketApi";

        private readonly FilePocketApiClient _apiClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthentictionRequests(
            FilePocketApiClient apiClient,
            AuthenticationStateProvider authenticationStateProvider,
            ILocalStorageService localStorage)
        {
            _apiClient = apiClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<TokenModel> LoginAsync(LoginModel loginModel)
        {
            var response = await _apiClient.PostAsJsonAsync(AuthUrl.LoginUrl, loginModel);

            if (!response.IsSuccessStatusCode)
            {
                return new TokenModel();
            }

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<TokenModel>(content)!;

            await _localStorage.SetItemAsync("authToken", result.Token);
            await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);

            ((AuthStateProvider)_authenticationStateProvider).NotifyUserAuthentication(loginModel.Email);

            _apiClient.SetBearerAuthorizationHeader(result.Token);

            return JsonConvert.DeserializeObject<TokenModel>(content)!;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("refreshToken");

            ((AuthStateProvider)_authenticationStateProvider).NotifyUserLogout();

            _apiClient.CleanUpAuthorizationHeader();
        }

        public async Task<TokenModel> RefreshAccessTokenAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");

            var tokenModel = new TokenModel()
            {
                Token = token!,
                RefreshToken = refreshToken!
            };

            var response = await _apiClient.PostAsJsonAsync(AuthUrl.RefreshTokenUrl, tokenModel);

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<TokenModel>(content)!;

            await _localStorage.SetItemAsync("authToken", result.Token);
            await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);

            _apiClient.SetBearerAuthorizationHeader(result.Token);

            return result;
        }

        public async Task<bool> RegisterUserAsync(RegistrationRequest registrationRequest)
        {
            var response = await _apiClient.PostAsJsonAsync(AuthUrl.AuthenticationUrl, registrationRequest);

            return response.IsSuccessStatusCode;
        }
    }
}
