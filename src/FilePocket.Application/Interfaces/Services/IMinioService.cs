using FilePocket.Domain.Models;
using FilePocket.Domain;
using Microsoft.AspNetCore.Http;

namespace FilePocket.Application.Interfaces.Services
{
    public interface IMinioService
    {
        Task<FileResponseModel> UploadFileAsync(
            IFormFile file,
            Guid userId, 
            Guid? pocketId, 
            FileTypes fileType, 
            Guid fileId, 
            CancellationToken cancellationToken = default);
        Task<FileResponseModel> DownloadFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
        Task<bool> RemoveFileAsync(Guid userId, Guid fileId, CancellationToken cancellationToken = default);
        Task<bool> DoesObjectExistAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
        Task CreateBucketIfNotExistsAsync(string bucketName, CancellationToken cancellationToken = default);
    }
}
