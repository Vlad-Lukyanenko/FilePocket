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

        Task<long> WriteObjectAsync(
            IFormFile file,
            string bucketName, 
            string objectName,
            CancellationToken cancellationToken = default);

        Task<long> WriteObjectAsync(
            byte[] data,
            string contentType,
            string bucketName,
            string objectName,
            CancellationToken cancellationToken = default);

        Task<byte[]> GetObjectAsBytesAsync(string bucketName, string objectName);
        Task<bool> DeleteObjectAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
        Task<bool> ObjectExistsAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
        Task CreateBucketIfNotExistsAsync(string bucketName, CancellationToken cancellationToken = default);
    }
}
