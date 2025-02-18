using Blazored.LocalStorage;
using FilePocket.BlazorClient.Features;
using FilePocket.BlazorClient.Features.Authentication;
using FilePocket.BlazorClient.Services.Authentication.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;

namespace FilePocket.BlazorClient.Services.Authentication.Requests
{
    public class AuthentictionRequests : IAuthentictionRequests
    {
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

        public Task<HttpResponseMessage> RegisterUserAsync(RegistrationRequest registrationRequest)
        {
            return _apiClient.PostAsJsonAsync(AuthUrl.AuthenticationUrl, registrationRequest);
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

            return JsonConvert.DeserializeObject<TokenModel>(content)!;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("refreshToken");

            ((AuthStateProvider)_authenticationStateProvider).NotifyUserLogout();

            _apiClient.CleanUpAuthorizationHeader();
        }
    }
}
