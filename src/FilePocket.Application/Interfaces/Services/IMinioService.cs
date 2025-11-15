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
            MinioActionArgs args,
            CancellationToken cancellationToken = default);

        Task<long> WriteObjectAsync(
            byte[] data,
            string contentType,
            MinioActionArgs args,
            CancellationToken cancellationToken = default);

        Task<byte[]> GetObjectAsBytesAsync(MinioActionArgs args);
        Task<bool> DeleteObjectAsync(MinioActionArgs args, CancellationToken cancellationToken = default);
        Task<bool> ObjectExistsAsync(MinioActionArgs args, CancellationToken cancellationToken = default);
        Task CreateBucketIfNotExistsAsync(string bucketName, CancellationToken cancellationToken = default);
    }
}
