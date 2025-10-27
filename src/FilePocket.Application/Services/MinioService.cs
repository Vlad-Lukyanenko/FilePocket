using FilePocket.Application.Exceptions;
using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain;
using FilePocket.Domain.Models;
using FilePocket.Domain.Models.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using System.IO;
using System.Net;


namespace FilePocket.Application.Services
{
    public class MinioService(IRepositoryManager repository,
        IMinioClient minioClient,
        IOptions<MinioConfigurationModel> options) : IMinioService
    {
        private readonly string _bucketName = options.Value.BucketName;
        private readonly IMinioClient _minioClient = minioClient;
        private readonly IRepositoryManager _repository = repository;

        public Task CreateBucketIfNotExistsAsync(string bucketName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveFileAsync(Guid userId, Guid fileId, CancellationToken cancellationToken = default)
        {
            var fileMetadata = await _repository.FileMetadata.GetByUserIdAndIdAsync(userId, fileId, trackChanges: true)
                ?? throw new FileMetadataNotFoundException(fileId);

            var objectPath = Path.Combine(fileMetadata.Path, fileMetadata.Id.ToString()).Replace("C:\\FilePocket\\", ""); // temporary for compatibility with current Path value

            var objectName = objectPath.Replace(Path.DirectorySeparatorChar, '/') // temporary for compatibility with current Path value
                .Replace(Path.AltDirectorySeparatorChar, '/');

            try
            {
                var bktExistArgs = new BucketExistsArgs()
                    .WithBucket(_bucketName);

                var args = new RemoveObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName);

                // versionId could be passed here laiter to delete specific object version

                await _minioClient!.RemoveObjectAsync(args, cancellationToken);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public Task<bool> DoesObjectExistAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponseModel> DownloadFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<FileResponseModel> UploadFileAsync(
                                                        IFormFile file,
                                                        Guid userId,
                                                        Guid? pocketId,
                                                        FileTypes fileType,
                                                        Guid fileId,
                                                        CancellationToken cancellationToken = default)
        {

            try
            {
                var bktExistArgs = new BucketExistsArgs()
                    .WithBucket(_bucketName);

                var found = await _minioClient!.BucketExistsAsync(bktExistArgs, cancellationToken).ConfigureAwait(false);
                if (!found)
                {
                    var mkBktArgs = new MakeBucketArgs()
                        .WithBucket(_bucketName);
                    await _minioClient.MakeBucketAsync(mkBktArgs, cancellationToken).ConfigureAwait(false);
                }

                var objectName = SelectObjectName(userId, pocketId, fileType, fileId);
                var contentType = file.ContentType;
                var contentLength = file.Length;

                using var stream = file.OpenReadStream();

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithContentType(contentType)
                    .WithObjectSize(contentLength);

                _ = await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            var fileResponse = new FileResponseModel
            {
                Id = fileId,
                UserId = userId,
                PocketId = pocketId,
                FileSize = file.Length / 1024,
                FileType = fileType,
                ActualName = "not implemented",
                OriginalName = file.FileName,
                CreatedAt = DateTime.UtcNow
            };

            return fileResponse;
        }

        private static string SelectObjectName(Guid userId, Guid? pocketId, FileTypes fileType, Guid fileId)
        {
            var now = DateTime.UtcNow;

            var objectName = pocketId is not null
                ? $"{userId}/{pocketId}/{now.Year}/{now.Month}/{fileType}s/{fileId}"
                : $"{userId}/{now.Year}/{now.Month}/{fileType}s/{fileId}";

            return objectName;
        }
    }
}
