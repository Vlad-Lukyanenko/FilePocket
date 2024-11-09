using FilePocket.Client.Features.Folders.Models;
using FilePocket.Client.Pages.Pockets;
using Newtonsoft.Json;
using System.Text;

namespace FilePocket.Client.Services.Folders.Requests
{
    public class FolderRequests : IFolderRequests
    {
        private const string HttpClientName = "FilePocketApi";

        private readonly HttpClient _httpClient;

        public FolderRequests(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient(HttpClientName);
        }

        public async Task<bool> CreateAsync(FolderModel folder)
        {
            var content = GetStringContent(folder);

            var response = await _httpClient.PostAsync("api/folders", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<FolderModel>> GetAllAsync(Guid pocketId, Guid parentFolderId)
        {
            var url = $"api/pockets/{pocketId}/parent-folder/{parentFolderId}/folders";
            
            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<FolderModel>>(content)!;
        }

        public async Task<IEnumerable<FolderModel>> GetAllAsync(Guid pocketId)
        {
            var url = $"api/pockets/{pocketId}/folders";
            
            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<FolderModel>>(content)!;
        }

        public async Task<bool> DeleteAsync(Guid folderId)
        {
            var url = $"api/folders/{folderId}";

            var response = await _httpClient.DeleteAsync(url);

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
}
