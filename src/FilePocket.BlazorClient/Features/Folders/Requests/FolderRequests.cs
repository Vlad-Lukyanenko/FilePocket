using FilePocket.BlazorClient.Features;
using FilePocket.BlazorClient.Features.Folders;
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

        var response = await _apiClient.PostAsync(FolderUrl.Create(), content);
        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            return false;
        }
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<FolderModel>> GetAllAsync(Guid? pocketId, Guid parentFolderId, List<FolderType> folderTypes, bool isSoftDeleted)
    {
        var folderTypeValues = folderTypes.Select(ft => (int)ft);
        var content = await _apiClient.GetAsync(FolderUrl.GetAll(pocketId, parentFolderId, isSoftDeleted, folderTypeValues));

        return JsonConvert.DeserializeObject<IEnumerable<FolderModel>>(content)!;
    }

    public async Task<FolderModel> GetAsync(Guid pocketId, Guid folderId)
    {
        var content = await _apiClient.GetAsync(FolderUrl.Get(pocketId, folderId));

        return JsonConvert.DeserializeObject<FolderModel>(content)!;
    }

    public async Task<IEnumerable<FolderModel>> GetAllAsync(Guid? pocketId, List<FolderType> folderTypes, bool isSoftDeleted)
    {
        var folderTypeValues = folderTypes.Select(ft => (int)ft);
        var content = await _apiClient.GetAsync(FolderUrl.GetAll(pocketId, isSoftDeleted, folderTypeValues));

        return JsonConvert.DeserializeObject<IEnumerable<FolderModel>>(content)!;
    }

    public async Task<bool> DeleteAsync(Guid folderId)
    {
        var response = await _apiClient.DeleteAsync(FolderUrl.Delete(folderId));

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> MoveToTrashAsync(Guid folderId)
    {
        var response = await _apiClient.PutAsync(FolderUrl.MoveToTrash(folderId));

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
