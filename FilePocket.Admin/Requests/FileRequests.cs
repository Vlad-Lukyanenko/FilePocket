using FilePocket.Admin.Models;
using FilePocket.Admin.Models.Files;
using FilePocket.Admin.Requests.Contracts;
using FilePocket.Admin.Requests.HttpRequests;
using FilePocket.Admin.Services;
using FilePocket.Domain.Models;
using Newtonsoft.Json;
using System.Text;
using static System.Net.WebRequestMethods;

namespace FilePocket.Admin.Requests
{
    public class FileRequests : IFileRequests
    {
        private readonly IHttpRequests _authorizedRequests;
        private readonly QueryStringConverter _queryConverter;

        public FileRequests(IHttpRequests authorizedRequests, QueryStringConverter queryConverter)
        {
            _authorizedRequests = authorizedRequests;
            _queryConverter = queryConverter;
        }

        public async Task<IEnumerable<FileModel>> GetAllAsync(Guid storageId)
        {
            var response = await _authorizedRequests.GetAsyncRequest($"api/files/all?storageId={storageId}");

            return await GetResult<IEnumerable<FileModel>>(response);
        }

        public async Task<FilteredFilesModel> GetFilteredAsync(Models.Files.FilesFilterOptionsModel filterOptions)
        {
            var query = _queryConverter.ToQueryString(filterOptions);
            var response = await _authorizedRequests.GetAsyncRequest($"api/files/filtered{query}");

            return await GetResult<FilteredFilesModel>(response);
        }

        public async Task<bool> CheckIfFileExists(string fileName, Guid storageId)
        {
            var response = await _authorizedRequests.GetAsyncRequest($"api/files/check?storageId={storageId}&fileName={fileName}");

            return await GetResult<bool>(response);
        }

        public async Task<ImageResponseModel> GetImageThumbnailAsync(Guid storageId, Guid imageId, int size)
        {
            var url = $"api/files/storage/{storageId}/image/thumbnail?id={imageId}&size={size}";
            var response = await _authorizedRequests.GetAsyncRequest(url);

            return await GetResult<ImageResponseModel>(response);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid storageId)
        {
            var response = await _authorizedRequests.DeleteAsyncRequest($"api/files/{id}?storageId={storageId}");

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutAsync(FileModel file, Guid storageId)
        {
            var response = await _authorizedRequests.PutAsyncRequest($"api/files?storageId={storageId}", file);

            return response.IsSuccessStatusCode;
        }

        public async Task<FileModel> PostAsync(MultipartFormDataContent content, Guid storageId)
        {
            var response = await _authorizedRequests.PostHttpContentAsyncRequest($"api/files/{storageId}", content);

            return await GetResult<FileModel>(response);
        }

        public async Task<FileDownloadModel> GetById(Guid id, Guid storageId)
        {
            var response = await _authorizedRequests.GetAsyncRequest($"api/files/{id}?storageId={storageId}");

            return await GetResult<FileDownloadModel>(response);
        }

        public async Task<SessionModel> CreateSession(Guid userId, Guid storageId, CreateSessionParams createParams)
        {
            var response = await _authorizedRequests.PostAsyncRequest($"api/files/session/user/{userId}/storage/{storageId}", createParams);

            return await GetResult<SessionModel>(response);

        }

        public async Task<bool> PostAsync(MultipartFormDataContent content, SessionModel session, int chunkNumber)
        {
            var response = await _authorizedRequests
                .PostHttpContentAsyncRequest($"api/files/upload/user/{session.UserId}/storage/{session.StorageId}/session/{session.Id}?chunkNumber={chunkNumber}", content);
            return response.IsSuccessStatusCode;
        }

        private static async Task<T> GetResult<T>(HttpResponseMessage response)
        {
            var result = await response.Content.ReadFromJsonAsync<T>();

            response.EnsureSuccessStatusCode();

            return result!;
        }
    }
}