using FilePocket.Domain;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Http;
using Minio;

namespace FilePocket.Application.Interfaces.Services
{
    public interface IMinioService
    {
        IMinioClient Client { get; }

        string BucketName { get; }

        Task<long> CreateObjectAsync(
            IFormFile file,
            string bucketName, 
            string objectName,
            CancellationToken cancellationToken = default);

        Task<byte[]> GetObjectAsBytesAsync(string bucketName, string objectName);
        Task<bool> DeleteObjectAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
        Task<bool> DoesObjectExistAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
        Task CreateBucketIfNotExistsAsync(string bucketName, CancellationToken cancellationToken = default);
    }
}
