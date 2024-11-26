using Blazored.LocalStorage;
using FilePocket.Client.Services.Authentication.Requests;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace FilePocket.Client.Features.Authentication
{
    public class AuthHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IAuthentictionRequests _authentictionRequests;

        public AuthHandler(
            ILocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider,
            IAuthentictionRequests authentictionRequests)
        {
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
            _authentictionRequests = authentictionRequests;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var expClaim = user.FindFirst(c => c.Type.Equals("exp"))!.Value;
            var expTime = DateTimeOffset.FromUnixTimeSeconds(
                Convert.ToInt64(expClaim));

            var diff = expTime - DateTime.UtcNow;

            var token = string.Empty;

            if(diff.TotalMinutes <= 14)
            {
                token = await _localStorage.GetItemAsync<string>("authToken");
            }
            else
            {
                token = (await _authentictionRequests.RefreshAccessTokenAsync()).Token;
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
