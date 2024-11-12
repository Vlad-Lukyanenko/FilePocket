using FilePocket.Client.Features.Files;
using FilePocket.Client.Services.Files.Models;
using Newtonsoft.Json;
using System.Net.Http.Json;

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
            var response = await _httpClient.GetAsync(FileUrl.GetFile(pocketId, fileId));

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<FileModel>(content)!;
        }

        public async Task<List<FileInfoModel>> GetFilesAsync(Guid pocketId)
        {
            var response = await _httpClient.GetAsync(FileUrl.GetAll(pocketId));

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<FileInfoModel>>(content)!;
        }

        public async Task<List<FileInfoModel>> GetFilesAsync(Guid pocketId, Guid folderId)
        {
            var response = await _httpClient.GetAsync(FileUrl.GetAll(pocketId, folderId));

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<FileInfoModel>>(content)!;
        }

        public async Task<FileModel> GetFileInfoAsync(Guid pocketId, Guid fileId)
        {
            var response = await _httpClient.GetAsync(FileUrl.GetFileInfo(pocketId, fileId));

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<FileModel>(content)!;
        }

        public async Task<FileModel> GetImageThumbnailAsync(Guid pocketId, Guid imageId, int size)
        {
            var response = await _httpClient.GetAsync(FileUrl.GetImageThumbnail(pocketId, imageId, size));

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<FileModel>(content)!;
        }

        public async Task<FileModel> UploadFileAsync(MultipartFormDataContent content, Guid pocketId)
        {
            var response = await _httpClient.PostAsync(FileUrl.UploadFile, content);

            var result = await response.Content.ReadFromJsonAsync<FileModel>();

            response.EnsureSuccessStatusCode();

            return result!;
        }

        public async Task DeleteFile(Guid pocketId, Guid fileId)
        {
            var response = await _httpClient.DeleteAsync(FileUrl.DeleteFile(pocketId, fileId));

            response.EnsureSuccessStatusCode();
        }
    }
}
