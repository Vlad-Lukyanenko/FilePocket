using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace FilePocket.Client.Features.Authentication;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly FilePocketApiClient _apiClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationState _anonymous;

    public AuthStateProvider(FilePocketApiClient apiClient, ILocalStorageService localStorage)
    {
        _apiClient = apiClient;
        _localStorage = localStorage;

        var anonymous = new ClaimsIdentity();

        _anonymous = new AuthenticationState(new ClaimsPrincipal(anonymous));
    }

    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(token))
        {
            return _anonymous;
        }

        _apiClient.SetBearerAuthorizationHeader(token);

        return new AuthenticationState(
                new ClaimsPrincipal(
                    new ClaimsIdentity(
                        JwtParser.ParseClaimsFromJwt(token), 
                        "jwtAuthType")));
    }

    public void NotifyUserAuthentication(string email)
    {
        var authenticatedUser = new ClaimsPrincipal(
            new ClaimsIdentity(
                [new Claim(ClaimTypes.Name, email)],
                "jwtAuthType"));

        var authState = Task.FromResult(
            new AuthenticationState(authenticatedUser));

        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(_anonymous);

        NotifyAuthenticationStateChanged(authState);
    }
}
