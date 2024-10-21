using FilePocket.Admin.Models.Authentication;
using FilePocket.Admin.Requests.Contracts;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;

namespace FilePocket.Admin.Extensions;

public static class HttpClientExtensions
{
    private const string AccessToken = "accessToken";
    private const string RefreshToken = "refreshToken";

    public static async Task AddAuthTokenFromStorageAsync(this HttpClient client, ProtectedLocalStorage protectedLocalStorage)
    {
        if (client.DefaultRequestHeaders.Authorization is null)
        {
            var accessToken = (await protectedLocalStorage.GetAsync<string>("accessToken")).Value!.Trim('"');
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }

    public static async Task<bool> RefreshTokensAsync(this HttpClient httpClient, ProtectedLocalStorage protectedLocalStorage, IAuthentictionRequests authRequests)
    {
        bool tokensRefreshed = true;
        var accessToken = (await protectedLocalStorage.GetAsync<string>(AccessToken)).Value!.Trim('"');
        var refreshToken = (await protectedLocalStorage.GetAsync<string>(RefreshToken)).Value!.Trim('"');
        var newTokens = await authRequests.RefreshAccessTokenAsync(new TokenModel() { AccessToken = accessToken, RefreshToken = refreshToken });

        if (string.IsNullOrEmpty(newTokens.AccessToken))
        {
            return !tokensRefreshed;
        }

        await protectedLocalStorage.SetAsync(AccessToken, newTokens.AccessToken!);
        await protectedLocalStorage.SetAsync(RefreshToken, newTokens.RefreshToken!);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newTokens.AccessToken);

        return tokensRefreshed;
    }
}
