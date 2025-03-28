using FilePocket.BlazorClient.Features.Bookmarks.Models;
using Newtonsoft.Json;
using System.Text;

namespace FilePocket.BlazorClient.Features.Bookmarks.Requests;

public class BookmarkRequests : IBookmarkRequests
{
    private readonly FilePocketApiClient _apiClient;
    private const string BaseUrl = "api/bookmark";

    public BookmarkRequests(FilePocketApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<BookmarkModel>> GetAllAsync(Guid? pocketId, Guid? folderId)
    {
        var url = string.Empty;

        if (pocketId is not null && folderId is not null)
        {
            url = $"api/pockets/{pocketId}/folders/{folderId}/bookmarks";
        }
        else if (pocketId is not null && folderId is null)
        {
            url = $"api/pockets/{pocketId}/bookmarks";
        }
        else
        {
            url = $"{BaseUrl}/all";
        }

        var content = await _apiClient.GetAsync(url);

        return JsonConvert.DeserializeObject<List<BookmarkModel>>(content)!;
    }

    public async Task<bool> CreateAsync(CreateBookmarkModel bookmark)
    {
        var content = GetStringContent(bookmark);

        var response = await _apiClient.PostAsync(BaseUrl, content);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(UpdateBookmarkModel bookmark)
    {
        var content = GetStringContent(bookmark);

        var response = await _apiClient.PutAsync(BaseUrl, content);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _apiClient.DeleteAsync($"{BaseUrl}/{id}");

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SoftDeleteAsync(Guid id)
    {
        var response = await _apiClient.DeleteAsync($"{BaseUrl}/soft/{id}");

        return response.IsSuccessStatusCode;
    }

    private static StringContent? GetStringContent(object? obj)
    {
        var json = JsonConvert.SerializeObject(obj);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
