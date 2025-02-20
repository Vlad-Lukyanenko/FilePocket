using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using FilePocket.BlazorClient.Services.Authentication;
using FilePocket.BlazorClient.Services.Authentication.Models;
using TusDotNetClient;


public class FilePocketDotNetTusClient
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;
    private readonly TusClient _tusClient;

    private string TusServerUrl => $"{BaseAddress}/tus/files";
    private string BaseAddress => _httpClient.BaseAddress?.OriginalString ?? "";

    public FilePocketDotNetTusClient(
        HttpClient httpClient,
        AuthenticationStateProvider authStateProvider,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
        _tusClient = new TusClient();
    }

    /// <summary>
    /// Ensures that the user is authenticated and the request contains a valid Bearer token.
    /// </summary>
    private async Task AuthenticateAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity is null || !user.Identity.IsAuthenticated)
        {
            return;
        }

        var expClaim = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
        if (string.IsNullOrEmpty(expClaim))
        {
            return;
        }

        var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(expClaim));
        var token = string.Empty;

        if (expTime > DateTime.UtcNow)
        {
            token = await _localStorage.GetItemAsync<string>("authToken");
        }
        else
        {
            token = (await RefreshAccessTokenAsync()).Token;
            await _localStorage.SetItemAsync("authToken", token);
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _tusClient.AdditionalHeaders.Add("Authorization", $"Bearer {token}");
    }

    /// <summary>
    /// Refreshes the JWT access token when expired.
    /// </summary>
    private async Task<TokenModel> RefreshAccessTokenAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");

        var tokenModel = new TokenModel
        {
            Token = token!,
            RefreshToken = refreshToken!
        };

        var response = await _httpClient.PostAsJsonAsync(AuthUrl.RefreshTokenUrl, tokenModel);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<TokenModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        await _localStorage.SetItemAsync("authToken", result.Token);
        await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);

        return result;
    }

    /// <summary>
    /// Securely creates an upload session with authentication.
    /// </summary>
    public async Task<string> CreateUploadSessionAsync(string fileName, long fileSize)
    {
        try
        {
            await AuthenticateAsync();

            return await _tusClient.CreateAsync(
                url: TusServerUrl,
                uploadLength: fileSize,
                ("filename", fileName)
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// Securely uploads a file in chunks with authentication.
    /// </summary>
    public async Task<bool> UploadFileAsync(string uploadUrl, Stream fileStream, Action<long, long> onProgress, CancellationToken cancellationToken)
    {
        await AuthenticateAsync();

        var uploadOperation = _tusClient.UploadAsync(
            url: uploadUrl,
            fileStream: fileStream,
            chunkSize: 5.0, // 5MB chunks
            cancellationToken: cancellationToken
        );

        uploadOperation.Progressed += (uploaded, total) => onProgress(uploaded, total);

        var responses = await uploadOperation; // Await completion

        // Check if all chunks were successfully uploaded
        return responses.All(response => response.StatusCode == System.Net.HttpStatusCode.NoContent);
    }
}
