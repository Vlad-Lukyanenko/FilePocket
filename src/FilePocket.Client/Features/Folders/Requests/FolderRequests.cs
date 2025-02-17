using FilePocket.Client.Features;
using FilePocket.Client.Features.Folders.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace FilePocket.Client.Services.Folders.Requests
{
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

            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<FolderModel>> GetAllAsync(Guid? pocketId, Guid parentFolderId)
        {

            var url = pocketId is null 
                ? $"api/parent-folder/{parentFolderId}/folders"
                : $"api/pockets/{pocketId}/parent-folder/{parentFolderId}/folders";
            
            var content = await _apiClient.GetAsync(url);

            return JsonConvert.DeserializeObject<IEnumerable<FolderModel>>(content)!;
        }

        public async Task<FolderModel> GetAsync(Guid folderId)
        { 
            var url = $"api/folders/{folderId}";
            
            var content = await _apiClient.GetAsync(url);

            return JsonConvert.DeserializeObject<FolderModel>(content)!;
        }

        public async Task<IEnumerable<FolderModel>> GetAllAsync(Guid? pocketId)
        {
            var url = pocketId is null 
                ? $"api/folders"
                : $"api/pockets/{pocketId}/folders";
            
            var content = await _apiClient.GetAsync(url);

            return JsonConvert.DeserializeObject<IEnumerable<FolderModel>>(content)!;
        }

        public async Task<bool> DeleteAsync(Guid folderId)
        {
            var url = $"api/folders/{folderId}";

            var response = await _apiClient.DeleteAsync(url);

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
        public async Task<bool> FolderExistsAsync(string folderName, Guid? pocketId, Guid? parentFolderId)
        {
            var query = $"api/folders/exists?folderName={folderName}&pocketId={pocketId}&parentFolderId={parentFolderId}";
            var response = await _apiClient.GetAsync(query);
            return bool.TryParse(response, out var exists) && exists;

        }
    }
}
