using FilePocket.Application.Exceptions;
using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using FilePocket.Domain.Models.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace FilePocket.Application.Services
{
    public class MinioService(IMinioClient minioClient,
        IOptions<MinioConfigurationModel> options) : IMinioService
    {
        private readonly string _bucketName = options.Value.BucketName;
        private readonly IMinioClient _minioClient = minioClient;

        public IMinioClient Client { get => _minioClient; }

        public string BucketName { get => _bucketName; }

        public Task CreateBucketIfNotExistsAsync(string bucketName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> GetObjectAsBytesAsync(string bucketName, string objectName)
        {
            using var memoryStream = new MemoryStream();

            var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream =>
                {
                    stream.CopyTo(memoryStream);
                });

            await Client.GetObjectAsync(args).ConfigureAwait(false);

            return memoryStream.ToArray();
        }

        public async Task<bool> DeleteObjectAsync(string bucketName,
            string objectName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var removeArgs = new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);

                // versionId could be passed here laiter to delete specific object version

                await _minioClient!.RemoveObjectAsync(removeArgs, cancellationToken);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> ObjectExistsAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
        {
            try
            {
                var statObjectArgs = new StatObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);

                var statObject = await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);
                return !statObject.ExtraHeaders.TryGetValue("x-amz-error-code", out var error) || error != "NoSuchKey";
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error checking object existence", e);
            }
        }

        public async Task<long> WriteObjectAsync(
            IFormFile file,
            string bucketName,
            string objectName,
            CancellationToken cancellationToken = default)
        {
            long uploadedFileSize = 0;

            try
            {
                var bktExistArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);

                var found = await _minioClient!.BucketExistsAsync(bktExistArgs, cancellationToken).ConfigureAwait(false);

                if (!found)
                {
                    var mkBktArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await _minioClient.MakeBucketAsync(mkBktArgs, cancellationToken).ConfigureAwait(false);
                }

                var contentType = file.ContentType;
                var contentLength = file.Length;

                using var stream = file.OpenReadStream();

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithContentType(contentType)
                    .WithObjectSize(contentLength);

                var response = await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken).ConfigureAwait(false);
                uploadedFileSize = response.Size;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return uploadedFileSize;
        }

        public async Task<long> WriteObjectAsync(
            byte[] fileBytes,
            string contentType,
            string bucketName,
            string objectName,
            CancellationToken cancellationToken = default)
        {
            long uploadedFileSize = 0;

            try
            {
                var bktExistArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);

                var found = await _minioClient!.BucketExistsAsync(bktExistArgs, cancellationToken).ConfigureAwait(false);

                if (!found)
                {
                    var mkBktArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await _minioClient.MakeBucketAsync(mkBktArgs, cancellationToken).ConfigureAwait(false);
                }

                var contentLength = fileBytes.Length;

                using var stream = new MemoryStream(fileBytes);

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithContentType(contentType)
                    .WithObjectSize(contentLength);

                var response = await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken).ConfigureAwait(false);
                uploadedFileSize = response.Size;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return uploadedFileSize;
        }

    }
}
