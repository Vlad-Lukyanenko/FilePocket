using FilePocket.Admin.Extensions;
using FilePocket.Admin.Requests.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using System.Net;

namespace FilePocket.Admin.Requests.HttpRequests;

public class HttpAuthorizedRequests : IHttpRequests
{
    private readonly HttpClient _httpClient;
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly NavigationManager _navigationManager;
    private readonly IAuthentictionRequests _authRequests;

    public HttpAuthorizedRequests(
        IHttpClientFactory factory,
        ProtectedLocalStorage protectedLocalStorage,
        NavigationManager navigationManager,
        IAuthentictionRequests authRequests,
        string httpClientName)
    {
        _httpClient = factory.CreateClient(httpClientName);
        _protectedLocalStorage = protectedLocalStorage;
        _navigationManager = navigationManager;
        _authRequests = authRequests;
    }

    public async Task<HttpResponseMessage> GetAsyncRequest(string requestUri)
    {
        await _httpClient.AddAuthTokenFromStorageAsync(_protectedLocalStorage);
        var response = await _httpClient.GetAsync(requestUri);

        await ProcessApiResponseAsync<object>(response);

        return response;
    }

    public async Task<HttpResponseMessage> PostAsyncRequest<TValue>(string requestUri, TValue value)
    {
        await _httpClient.AddAuthTokenFromStorageAsync(_protectedLocalStorage);
        var response = await _httpClient.PostAsJsonAsync(requestUri, value);

        await ProcessApiResponseAsync<TValue>(response);

        return response;
    }

    public async Task<HttpResponseMessage> PostHttpContentAsyncRequest<TValue>(string requestUri, TValue content) where TValue : HttpContent
    {
        await _httpClient.AddAuthTokenFromStorageAsync(_protectedLocalStorage);
        var response = await _httpClient.PostAsync(requestUri, content);

        await ProcessApiResponseAsync<TValue>(response);

        return response;
    }


    public async Task<HttpResponseMessage> PutAsyncRequest<TValue>(string requestUri, TValue value)
    {
        await _httpClient.AddAuthTokenFromStorageAsync(_protectedLocalStorage);
        var response = await _httpClient.PutAsJsonAsync(requestUri, value);

        await ProcessApiResponseAsync<TValue>(response);

        return response;
    }

    public async Task<HttpResponseMessage> DeleteAsyncRequest(string requestUri)
    {
        await _httpClient.AddAuthTokenFromStorageAsync(_protectedLocalStorage);
        var response = await _httpClient.DeleteAsync(requestUri);

        await ProcessApiResponseAsync<object>(response);

        return response;
    }

    private async Task ProcessApiResponseAsync<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    var isRefreshed = await _httpClient.RefreshTokensAsync(_protectedLocalStorage, _authRequests);
                    if (!isRefreshed)
                    {
                        _navigationManager.NavigateTo("/", true);
                    }
                    else
                    {
                        await ReRequest<T>(response);
                    }
                    break;
                case HttpStatusCode.Forbidden:
                    _navigationManager.NavigateTo("/forbidden", true);
                    break;
                default:
                    _navigationManager.NavigateTo("/error", true);
                    break;
            }
        }
    }

    private async Task ReRequest<T>(HttpResponseMessage response)
    {
        string requestContent = string.Empty;
        var methodType = response.RequestMessage!.Method.ToString();
        var requestUri = response.RequestMessage.RequestUri!.AbsolutePath;

        if (methodType == "POST" || methodType == "PUT")
        {
            requestContent = await response.RequestMessage.Content!.ReadAsStringAsync();
        }

        response = methodType switch
        {
            "GET" => await GetAsyncRequest(requestUri),
            "POST" => await PostAsyncRequest(requestUri, JsonConvert.DeserializeObject<T>(requestContent)),
            "PUT" => await PutAsyncRequest(requestUri, JsonConvert.DeserializeObject<T>(requestContent)),
            "DELETE" => await DeleteAsyncRequest(requestUri),
            _ => new HttpResponseMessage(HttpStatusCode.BadRequest)
        };
    }
}
