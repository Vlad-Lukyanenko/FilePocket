using FilePocket.Client.Services.Files.Models;
using Newtonsoft.Json;

namespace FilePocket.Client.Services.Files.Requests
{
    public class FileRequests : IFileRequests
    {
        private const string HttpClientName = "FilePocketApi";

        private readonly HttpClient _httpClient;


        public FileRequests(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient(HttpClientName);
        }

        public async Task<FileModel> GetFileAsync(Guid pocketId, Guid fileId)
        {
            var response = await _httpClient.GetAsync($"api/files/{fileId}/storages/{pocketId}");

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<FileModel>(content)!;
        }

        public async Task<List<FileInfoModel>> GetFilesAsync(Guid pocketId)
        {
            var response = await _httpClient.GetAsync($"api/files/pocket/{pocketId}");

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<FileInfoModel>>(content)!;
        }
    }
}
