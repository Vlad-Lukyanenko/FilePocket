using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace FilePocket.Client.Features.Authentication
{
    public class AuthHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public AuthHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = (await _localStorage.GetItemAsync<string>("authToken"));

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
