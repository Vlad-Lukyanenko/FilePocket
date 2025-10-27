using FilePocket.Application.Exceptions;
using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;
using System.IO;
using System.Net;


namespace FilePocket.Application.Services
{
    public class MinioService : IMinioService
    {
        private readonly string _endpoint = "localhost:9000";
        private readonly string _accessKey = "myminioadmin";
        private readonly string _secretKey = "minio-secret-key";
        private readonly string _bucketName = "file-pocket";

        private readonly IMinioClient? _minioClient;
        private readonly IRepositoryManager _repository;

        public MinioService(IRepositoryManager repositoryManager)
        {
            _minioClient = new MinioClient()
                .WithEndpoint(_endpoint)
                .WithCredentials(_accessKey, _secretKey)
                .Build();

            _repository = repositoryManager;
        }

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
