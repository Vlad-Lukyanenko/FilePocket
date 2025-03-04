﻿using FilePocket.BlazorClient.Features;
using FilePocket.BlazorClient.Features.Files;
using FilePocket.BlazorClient.Helpers;
using FilePocket.BlazorClient.Services.Files.Models;
using Newtonsoft.Json;
using System.Net.Http.Json;
using FilePocket.BlazorClient.Shared.Models;
using FilePocket.BlazorClient.Features.Files.Models;

namespace FilePocket.BlazorClient.Services.Files.Requests
{
    public class FileRequests : IFileRequests
    {
        private readonly FilePocketApiClient _apiClient;

        public FileRequests(FilePocketApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<FileModel> GetFileAsync(Guid fileId)
        {
            var url = FileUrl.GetFile(fileId);

            var content = await _apiClient.GetAsync(url);

            return JsonConvert.DeserializeObject<FileModel>(content)!;
        }

        public async Task<List<FileInfoModel>> GetFilesAsync(Guid? pocketId, Guid? folderId)
        {
            var url = string.Empty;

            if (pocketId is not null && folderId is not null)
            {
                url = FileUrl.GetAll(pocketId.Value, folderId.Value);
            }
            else if (pocketId is null && folderId is not null)
            {
                url = FileUrl.GetAllFromFolder(folderId.Value);
            }
            else if (pocketId is not null && folderId is null)
            {
                url = FileUrl.GetAll(pocketId.Value);
            }
            else if (pocketId is null && folderId is null)
            {
                url = FileUrl.GetAll();
            }

            var content = await _apiClient.GetAsync(url);

            return JsonConvert.DeserializeObject<List<FileInfoModel>>(content)!;
        }

        public async Task<FileModel> GetFileInfoAsync(Guid fileId)
        {
            var content = await _apiClient.GetAsync(FileUrl.GetFileInfo(fileId));

            return JsonConvert.DeserializeObject<FileModel>(content)!;
        }

        public async Task<FileModel> GetImageThumbnailAsync(Guid imageId, int size)
        {
            var content = await _apiClient.GetAsync(FileUrl.GetImageThumbnail(imageId, size));
            var result = JsonConvert.DeserializeObject<FileModel>(content)!;
            string base64 = Convert.ToBase64String(new ReadOnlySpan<byte>(result.FileByteArray!));
            var mimeType = Tools.GetMimeType(result.OriginalName!);
            result.FileData = $"data:{mimeType};base64,{base64}";

            var fileToCache = new ThumbnailRecord()
            {
                Id = result.Id,
                PocketId= result.PocketId,
                OriginalName= result.OriginalName,
                FileSize= result.FileSize,
                DataUrl = result.FileData
            };

            return result;
        }

        public async Task<FileModel> UploadFileAsync(MultipartFormDataContent content)
        {
            var response = await _apiClient.PostAsync(FileUrl.UploadFile, content);

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorDetails = await ErrorDetailsModel.UnwrapErrorAsync(response);
               errorDetails.Throw();
            }

            var result = await response.Content.ReadFromJsonAsync<FileModel>();

            response.EnsureSuccessStatusCode();

            var fileToCache = new ThumbnailRecord()
            {
                Id = result.Id,
                PocketId= result.PocketId,
                OriginalName= result.OriginalName,
                FileSize= result.FileSize,
                DataUrl = result.FileData
            };

            //await _thumbnailCacheService.AddThumbnailAsync(fileToCache);

            return result!;
        }

        public async Task DeleteFile(Guid fileId)
        {
            var response = await _apiClient.DeleteAsync(FileUrl.DeleteFile(fileId));

            response.EnsureSuccessStatusCode();
        }

        public async Task<List<FileInfoModel>> GetRecentFilesAsync()
        {
            var content = await _apiClient.GetAsync(FileUrl.GetRecentFiles());

            return JsonConvert.DeserializeObject<List<FileInfoModel>>(content)!;
        }

        public Task<List<FileInfoModel>> GetFilesAsync(Guid? pocketId)
        {
            throw new NotImplementedException();
        }
    }
}
