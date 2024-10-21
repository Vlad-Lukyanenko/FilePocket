using FilePocket.Admin.Models.Authentication;
using FilePocket.Admin.Requests.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace FilePocket.Admin.AuthFeatures;

public class AuthStateProvider : AuthenticationStateProvider
{
    private const string AccessToken = "accessToken";
    private const string RefreshToken = "refreshToken";
    private readonly IAuthentictionRequests _authRequests;
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly NavigationManager _navigationManager;

    public AuthStateProvider(IAuthentictionRequests authRequests, ProtectedLocalStorage protectedLocalStorage, NavigationManager navigationManager)
    {
        _authRequests = authRequests;
        _protectedLocalStorage = protectedLocalStorage;
        _navigationManager = navigationManager;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = new ClaimsPrincipal();

        ProtectedBrowserStorageResult<string> storedAccessToken = new (); 
        ProtectedBrowserStorageResult<string> storedRefreshToken = new ();

        try
        {
            storedAccessToken = await _protectedLocalStorage.GetAsync<string>(AccessToken);
            storedRefreshToken = await _protectedLocalStorage.GetAsync<string>(RefreshToken);
        }
        catch (Exception)
        {
            //TODO: do nothing
        }

        if (storedAccessToken.Success && storedRefreshToken.Success)
        {
            var claims = JwtParser.ParseClaimsFromJwt(storedAccessToken.Value!);
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            principal = new ClaimsPrincipal(identity);

            var tokenExp = claims.First(claim => claim.Type.Equals("exp")).Value;
            var expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(tokenExp)).UtcDateTime;

            if (expDate < DateTime.UtcNow)
            {
                var tokens = await _authRequests.RefreshAccessTokenAsync(new TokenModel
                {
                    AccessToken = storedAccessToken.Value!.Trim('"'),
                    RefreshToken = storedRefreshToken.Value!.Trim('"')
                });

                if (string.IsNullOrEmpty(tokens.AccessToken))
                {
                    await DeleteTokensFromStorageAsync();
                    _navigationManager.NavigateTo("/");

                    return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
                }

                await SetTokensToStorageAsync(tokens);

                var freshClaims = JwtParser.ParseClaimsFromJwt(tokens.AccessToken!);
                identity = new ClaimsIdentity(freshClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                principal = new ClaimsPrincipal(identity);
            }
        }

        return await Task.FromResult(new AuthenticationState(principal));
    }

    public async Task<bool> LoginAsync(LoginModel loginModel)
    {
        var apiTokens = await _authRequests.LoginUserAsync(new LoginModel { Email = loginModel.Email, Password = loginModel.Password });

        if (string.IsNullOrEmpty(apiTokens.AccessToken))
        {
            return false;
        }

        var principal = new ClaimsPrincipal();

        var claims = JwtParser.ParseClaimsFromJwt(apiTokens.AccessToken!);
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        principal = new ClaimsPrincipal(identity);

        await SetTokensToStorageAsync(apiTokens);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));

        return true;
    }

    public async Task LogoutAsync()
    {
        await DeleteTokensFromStorageAsync();

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
    }

    private async Task DeleteTokensFromStorageAsync()
    {
        await _protectedLocalStorage.DeleteAsync(AccessToken);
        await _protectedLocalStorage.DeleteAsync(RefreshToken);
    }

    private async Task SetTokensToStorageAsync(TokenModel apiTokens)
    {
        await _protectedLocalStorage.SetAsync(AccessToken, apiTokens.AccessToken!);
        await _protectedLocalStorage.SetAsync(RefreshToken, apiTokens.RefreshToken!);
    }
}
