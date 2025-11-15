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

        public async Task<byte[]> GetObjectAsBytesAsync(MinioActionArgs args)
        {
            using var memoryStream = new MemoryStream();

            var objectArgs = new GetObjectArgs()
                .WithBucket(args.BucketName)
                .WithObject(args.ObjectName)
                .WithCallbackStream(stream =>
                {
                    stream.CopyTo(memoryStream);
                });

            await Client.GetObjectAsync(objectArgs).ConfigureAwait(false);

            return memoryStream.ToArray();
        }

        public async Task<bool> DeleteObjectAsync(MinioActionArgs args,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var removeArgs = new RemoveObjectArgs()
                    .WithBucket(args.BucketName)
                    .WithObject(args.ObjectName);

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

        public async Task<bool> ObjectExistsAsync(MinioActionArgs args, CancellationToken cancellationToken = default)
        {
            try
            {
                var statObjectArgs = new StatObjectArgs()
                    .WithBucket(args.BucketName)
                    .WithObject(args.ObjectName);

                var statObject = await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);

                return !statObject.ExtraHeaders.TryGetValue("X-Minio-Error-Code", out var error) || error != "NoSuchKey";
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error checking object existence", e);
            }
        }

        public async Task<long> WriteObjectAsync(
            IFormFile file,
            MinioActionArgs args,
            CancellationToken cancellationToken = default)
        {
            long uploadedFileSize = 0;

            try
            {
                var bktExistArgs = new BucketExistsArgs()
                    .WithBucket(args.BucketName);

                var found = await _minioClient!.BucketExistsAsync(bktExistArgs, cancellationToken).ConfigureAwait(false);

                if (!found)
                {
                    var mkBktArgs = new MakeBucketArgs()
                        .WithBucket(args.BucketName);
                    await _minioClient.MakeBucketAsync(mkBktArgs, cancellationToken).ConfigureAwait(false);
                }

                var contentType = file.ContentType;
                var contentLength = file.Length;

                using var stream = file.OpenReadStream();

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(args.BucketName)
                    .WithObject(args.ObjectName)
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
            MinioActionArgs args,
            CancellationToken cancellationToken = default)
        {
            long uploadedFileSize = 0;

            try
            {
                var bktExistArgs = new BucketExistsArgs()
                    .WithBucket(args.BucketName);

                var found = await _minioClient!.BucketExistsAsync(bktExistArgs, cancellationToken).ConfigureAwait(false);

                if (!found)
                {
                    var mkBktArgs = new MakeBucketArgs()
                        .WithBucket(args.BucketName);
                    await _minioClient.MakeBucketAsync(mkBktArgs, cancellationToken).ConfigureAwait(false);
                }

                var contentLength = fileBytes.Length;

                using var stream = new MemoryStream(fileBytes);

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(args.BucketName)
                    .WithObject(args.ObjectName)
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
