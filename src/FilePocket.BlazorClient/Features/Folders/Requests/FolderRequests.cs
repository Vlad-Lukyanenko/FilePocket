using FilePocket.BlazorClient.Features;
using FilePocket.BlazorClient.Features.Folders.Models;
using FilePocket.BlazorClient.Shared.Enums;
using Newtonsoft.Json;
using System.Text;

namespace FilePocket.BlazorClient.Services.Folders.Requests;

public class FolderRequests : IFolderRequests
{
    private readonly FilePocketApiClient _apiClient;

    public FolderRequests(FilePocketApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<bool> CreateAsync(FolderModel folder)
    {
        var content = GetStringContent(folder);

        var response = await _apiClient.PostAsync("api/folders", content);
        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            return false;
        }
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<FolderModel>> GetAllAsync(Guid? pocketId, Guid parentFolderId, List<FolderType> folderTypes, bool isSoftDeleted)
    {
        var folderTypesQueryStringParams = string.Concat("?folderTypes=", string.Join("&folderTypes=", folderTypes));

        var url = pocketId is null 
            ? $"api/parent-folder/{parentFolderId}/{isSoftDeleted}/folders{folderTypesQueryStringParams}"
            : $"api/pockets/{pocketId}/parent-folder/{parentFolderId}/{isSoftDeleted}/folders{folderTypesQueryStringParams}";
        
        var content = await _apiClient.GetAsync(url);

        return JsonConvert.DeserializeObject<IEnumerable<FolderModel>>(content)!;
    }

    public async Task<FolderModel> GetAsync(Guid pocketId, Guid folderId)
    { 
        var url = $"api/pockets/{pocketId}/folders/{folderId}";
            
        var content = await _apiClient.GetAsync(url);

        return JsonConvert.DeserializeObject<FolderModel>(content)!;
    }

    public async Task<IEnumerable<FolderModel>> GetAllAsync(Guid? pocketId, List<FolderType> folderTypes, bool isSoftDeleted)
    {
        var folderTypesQueryStringParams = string.Concat("?folderTypes=", string.Join("&folderTypes=", folderTypes));

        var url = pocketId is null 
            ? $"api/folders/{isSoftDeleted}{folderTypesQueryStringParams}"
            : $"api/pockets/{pocketId}/{isSoftDeleted}/folders{folderTypesQueryStringParams}";
        
        var content = await _apiClient.GetAsync(url);

        return JsonConvert.DeserializeObject<IEnumerable<FolderModel>>(content)!;
    }

    public async Task<bool> DeleteAsync(Guid folderId)
    {
        var url = $"api/folders/{folderId}";

        var response = await _apiClient.DeleteAsync(url);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SoftDeleteAsync(Guid folderId)
    {
        var url = $"api/folders/soft/{folderId}";

        var response = await _apiClient.DeleteAsync(url);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RestoreAsync(Guid folderId)
    {
        var url = $"api/folders/restore/{folderId}";

        var response = await _apiClient.PutAsync(url, null);

        return response.IsSuccessStatusCode;
    }

    public Task<bool> UpdateAsync(FolderModel folder)
    {
        throw new NotImplementedException();
    }

    private static StringContent? GetStringContent(object? obj)
    {
        var json = JsonConvert.SerializeObject(obj);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
