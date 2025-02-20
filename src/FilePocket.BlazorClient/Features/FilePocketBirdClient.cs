using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BirdMessenger;
using BirdMessenger.Collections;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using FilePocket.BlazorClient.Services.Authentication;
using FilePocket.BlazorClient.Services.Authentication.Models;

public class FilePocketBirdClient(
    HttpClient httpClient,
    ITusClient tusClient,
    AuthenticationStateProvider authStateProvider,
    ILocalStorageService localStorage)
{
    private string TusServerUrl => $"{BaseAddress}/tus/files";
    private string BaseAddress => httpClient.BaseAddress?.OriginalString ?? "";

    /// <summary>
    /// Ensures the request contains a valid JWT Bearer token.
    /// </summary>
    private async Task AuthenticateAsync()
    {
        var authState = await authStateProvider.GetAuthenticationStateAsync();
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
            token = await localStorage.GetItemAsync<string>("authToken");
        }
        else
        {
            token = (await RefreshAccessTokenAsync()).Token;
            await localStorage.SetItemAsync("authToken", token);
        }

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Refreshes the JWT access token when expired.
    /// </summary>
    private async Task<TokenModel> RefreshAccessTokenAsync()
    {
        var refreshToken = await localStorage.GetItemAsync<string>("refreshToken");

        var tokenModel = new TokenModel
        {
            RefreshToken = refreshToken!
        };

        var response = await httpClient.PostAsJsonAsync(AuthUrl.RefreshTokenUrl, tokenModel);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<TokenModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        await localStorage.SetItemAsync("authToken", result.Token);
        await localStorage.SetItemAsync("refreshToken", result.RefreshToken);

        return result;
    }

    /// <summary>
    /// Securely creates an upload session with JWT authentication.
    /// </summary>
    public async Task<TusCreateResponse> CreateUploadSessionAsync(long fileSize, string fileName)
    {
        await AuthenticateAsync();

        var tusClient = new TusClient(httpClient);

        var metadata = new MetadataCollection
        {
            { "filename", fileName }
        };

        var requestOption = new TusCreateRequestOption
        {
            UploadLength = fileSize,
            Metadata = metadata,
            Endpoint = new Uri(TusServerUrl, UriKind.Absolute)
        };

        var tusCreateResponse = await tusClient.TusCreateAsync(requestOption, CancellationToken.None);
        return tusCreateResponse;
    }

    /// <summary>
    /// Securely uploads a file in chunks with JWT authentication.
    /// </summary>
    public async Task<bool> UploadFileAsync(TusCreateResponse response, Stream fileStream, Action<long, long> onProgress, CancellationToken cancellationToken)
    {
        var tusPatchRequestOption = new TusPatchRequestOption
        {
            FileLocation = response.FileLocation,
            Stream = fileStream,
            UploadBufferSize = 5*1024*1024, // upload size ,default value is 1MB
            UploadType = UploadType.Chunk,  // setting upload file with Stream or chunk ,default value is Stream
            OnProgressAsync = x =>
            {
                var uploadedProgress = (int)Math.Floor(100 * (double)x.UploadedSize / x.TotalSize);
                onProgress(x.UploadedSize, x.TotalSize);
                Console.WriteLine($"OnProgressAsync-TotalSize:{x.TotalSize}-UploadedSize:{x.UploadedSize}-uploadedProgress:{uploadedProgress}");
                return Task.CompletedTask;
            },
            OnCompletedAsync = x =>
            {
                var reqOption = x.TusRequestOption as TusPatchRequestOption;
                Console.WriteLine($"File:{reqOption.FileLocation} Completed ");
                return Task.CompletedTask;
            },
            OnFailedAsync = x =>
            {
                Console.WriteLine($"error： {x.Exception.Message}");
                if (x.OriginHttpRequestMessage is not null)
                {
                    //log httpRequest
                }

                if (x.OriginResponseMessage is not null)
                {
                    //log response
                }
                return Task.CompletedTask;
            }
        };

        var tusPatchResp = await tusClient.TusPatchAsync(tusPatchRequestOption, CancellationToken.None);
        return tusPatchResp.OriginResponseMessage.IsSuccessStatusCode;
    }
}
