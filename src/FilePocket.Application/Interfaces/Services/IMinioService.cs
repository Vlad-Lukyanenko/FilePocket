using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace FilePocket.Application.Interfaces.Services
{
    public interface IMinioService
    {
        Task UploadFileAsync(IFormFile file, string bucketName, string objectName, string contentType, CancellationToken cancellationToken = default);
        Task<FileResponseModel> DownloadFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
        Task DeleteFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
        Task<bool> DoesObjectExistAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
        Task CreateBucketIfNotExistsAsync(string bucketName, CancellationToken cancellationToken = default);
    }
}
