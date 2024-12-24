using FilePocket.Client.Features;
using FilePocket.Client.Features.Files;
using FilePocket.Client.Services.Files.Models;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace FilePocket.Client.Services.Files.Requests
{
    public class FileRequests : IFileRequests
    {
        private readonly FilePocketApiClient _apiClient;

        public FileRequests(FilePocketApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<FileModel> GetFileAsync(Guid pocketId, Guid fileId)
        {
            var content = await _apiClient.GetAsync(FileUrl.GetFile(pocketId, fileId));

            return JsonConvert.DeserializeObject<FileModel>(content)!;
        }

        public async Task<List<FileInfoModel>> GetFilesAsync(Guid pocketId)
        {
            var content = await _apiClient.GetAsync(FileUrl.GetAll(pocketId));

            return JsonConvert.DeserializeObject<List<FileInfoModel>>(content)!;
        }

        public async Task<List<FileInfoModel>> GetFilesAsync(Guid pocketId, Guid folderId)
        {
            var content = await _apiClient.GetAsync(FileUrl.GetAll(pocketId, folderId));

            return JsonConvert.DeserializeObject<List<FileInfoModel>>(content)!;
        }

        public async Task<FileModel> GetFileInfoAsync(Guid pocketId, Guid fileId)
        {
            var content = await _apiClient.GetAsync(FileUrl.GetFileInfo(pocketId, fileId));

            return JsonConvert.DeserializeObject<FileModel>(content)!;
        }

        public async Task<FileModel> GetImageThumbnailAsync(Guid pocketId, Guid imageId, int size)
        {
            var content = await _apiClient.GetAsync(FileUrl.GetImageThumbnail(pocketId, imageId, size));

            return JsonConvert.DeserializeObject<FileModel>(content)!;
        }

        public async Task<FileModel> UploadFileAsync(MultipartFormDataContent content, Guid pocketId)
        {
            var response = await _apiClient.PostAsync(FileUrl.UploadFile, content);

            var result = await response.Content.ReadFromJsonAsync<FileModel>();

            response.EnsureSuccessStatusCode();

            return result!;
        }

        public async Task DeleteFile(Guid pocketId, Guid fileId)
        {
            var response = await _apiClient.DeleteAsync(FileUrl.DeleteFile(pocketId, fileId));

            response.EnsureSuccessStatusCode();
        }
    }
}
