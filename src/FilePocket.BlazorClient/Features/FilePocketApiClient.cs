using Blazored.LocalStorage;
using FilePocket.BlazorClient.Services.Authentication;
using FilePocket.BlazorClient.Services.Authentication.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FilePocket.BlazorClient.Features
{
    public class FilePocketApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;
        
        public FilePocketApiClient(
            HttpClient httpClient,
            AuthenticationStateProvider authStateProvider,
            ILocalStorageService localStorage)
        {
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
            _httpClient = httpClient;
        }

        public string BaseAddress => _httpClient.BaseAddress.OriginalString;

        public async Task<HttpResponseMessage> PostAsJsonAsync(string endpoint, object model)
        {
            await AuthAsync();

            return await _httpClient.PostAsJsonAsync(endpoint, model);
        }

        public async Task<HttpResponseMessage> PostAsync(string endpoint, StringContent? content)
        {
            await AuthAsync();

            return await _httpClient.PostAsync(endpoint, content);
        }
        
        public async Task<HttpResponseMessage> PostAsync(string endpoint, MultipartFormDataContent? content)
        {
            await AuthAsync();

            return await _httpClient.PostAsync(endpoint, content);
        }

        public async Task<HttpResponseMessage> PutAsync(string endpoint, StringContent? content)
        {
            await AuthAsync();

            return await _httpClient.PutAsync(endpoint, content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            await AuthAsync();

            return await _httpClient.DeleteAsync(endpoint);
        }

        public async Task<string> GetAsync(string endpoint)
        {
            await AuthAsync();

            var response = await _httpClient.GetAsync(endpoint);

            return await response.Content.ReadAsStringAsync();
        }

        public void CleanUpAuthorizationHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        private async Task AuthAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();

            var user = authState.User;

            if (user.Identity is null || !user.Identity.IsAuthenticated)
            {
                return;
            }

            var expClaim = user.FindFirst(c => c.Type.Equals("exp"))!.Value;
            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(expClaim));

            var token = string.Empty;

            if(expTime > DateTime.Now)
            {
                token = await _localStorage.GetItemAsync<string>("authToken");
            }
            else
            {
                token = (await RefreshAccessTokenAsync()).Token;
                await _localStorage.SetItemAsync("authToken", token);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<TokenModel> RefreshAccessTokenAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");

            var tokenModel = new TokenModel()
            {
                Token = token!,
                RefreshToken = refreshToken!
            };

            var response = await _httpClient.PostAsJsonAsync(AuthUrl.RefreshTokenUrl, tokenModel);

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<TokenModel>(content)!;

            await _localStorage.SetItemAsync("authToken", result.Token);
            await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);

            return result;
        }
    }
}
