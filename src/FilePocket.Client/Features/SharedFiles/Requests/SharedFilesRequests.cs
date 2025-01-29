using FilePocket.Client.Features.SharedFiles.Models;
using Newtonsoft.Json;
using System.Text;

namespace FilePocket.Client.Features.SharedFiles.Requests
{
    public class SharedFilesRequests : ISharedFilesRequests
    {
        private readonly FilePocketApiClient _apiClient;

        public string BaseAddress => _apiClient.BaseAddress;

        public SharedFilesRequests(FilePocketApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<Guid> CreateAsync(SharedFileModel sharedFile)
        {
            var url = $"api/files/shared";
            
            var json = JsonConvert.SerializeObject(sharedFile);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _apiClient.PostAsync(url, content);

            return sharedFile.Id;            
        }

        public async Task<SharedFileModel?> GetByIdAsync(Guid sharedFileId)
        {
            var url = $"api/files/shared/{sharedFileId}";
            
            var content = await _apiClient.GetAsync(url);

            return JsonConvert.DeserializeObject<SharedFileModel>(content)!;
        }

        public async Task<byte[]?> DownloadAsync(Guid sharedFileId)
        {
            var url = $"api/files/shared/download/{sharedFileId}";
            
            var content = await _apiClient.GetAsync(url);

            return JsonConvert.DeserializeObject<byte[]>(content)!;
        }

        public async Task<List<SharedFileView>> GetAllAsync()
        {
            var url = $"api/files/shared";
            
            var content = await _apiClient.GetAsync(url);

            return JsonConvert.DeserializeObject<List<SharedFileView>>(content)!;
        }

        public async Task<List<SharedFileView>> GetLatestAsync()
        {
            var url = $"api/files/shared/latest";
            
            var content = await _apiClient.GetAsync(url);

            return JsonConvert.DeserializeObject<List<SharedFileView>>(content)!;
        }

        public async Task<bool> DeleteAsync(Guid sharedFileId)
        {
            var url = $"api/files/shared/{sharedFileId}";

            var response = await _apiClient.DeleteAsync(url);

            return response.IsSuccessStatusCode;
        }
    }
}
