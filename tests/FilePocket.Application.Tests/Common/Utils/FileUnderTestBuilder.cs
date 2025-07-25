﻿using System.Net.Http.Headers;
using FilePocket.Application.IntegrationTests.Api.Files.Models;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Controllers;

namespace FilePocket.Application.IntegrationTests.Common.Utils;

public static class FileUnderTestBuilder
{
    public static FileInformationModelUnderTest[] CreateMany(
        long totalStorageCapacityInMegabytes,
        int filesAmount,
        string fileName,
        string fileExtension,
        Guid userId,
        Guid pocketId,
        Guid? folderId = null)
    {
        if (totalStorageCapacityInMegabytes <= 0)
            throw new ArgumentException("The total storage capacity must be greater than zero.", nameof(totalStorageCapacityInMegabytes));
        
        if (filesAmount <= 0)
            throw new ArgumentException("The files amount must be greater than zero.", nameof(filesAmount));

        var sizeInBytesPerFile = totalStorageCapacityInMegabytes.GetFileSizeInBytes() / filesAmount * 1.1;
        if (sizeInBytesPerFile * filesAmount < totalStorageCapacityInMegabytes.GetFileSizeInBytes())
            throw new ArgumentException("The total storage capacity must be divisible by the files amount.", nameof(totalStorageCapacityInMegabytes));

        var files = new FileInformationModelUnderTest[filesAmount];
        for (var i = 0; i < filesAmount; i++)
        {
            var sequentialFileName = $"[{i}]-{fileName}";
            files[i] = CreateOne(sizeInBytesPerFile, sequentialFileName, fileExtension, userId, pocketId, folderId);
        }

        return files;
    }
    
    /// <summary>
    /// Creates a MultipartFormDataContent containing a simulated file and additional fields.
    /// </summary>
    /// <param name="fileSizeInBytes">The size of the file in bytes.</param>
    /// <returns>A MultipartFormDataContent object that can be used in an HTTP POST.</returns>
    public static FileInformationModelUnderTest CreateOne(
        double fileSizeInBytes,
        string fileName,
        string fileExtension,
        Guid userId,
        Guid pocketId,
        Guid? folderId = null)
    {
        var multipartContent = CreateMultipartFormDataContent(
            fileSizeInBytes, fileName, fileExtension,pocketId, folderId);

        var fileUnderTest = new FileInformationModelUnderTest
        {
            UserId = userId,
            FileName = fileName,
            FileExtension = fileExtension,
            FileSizeInBytes = (long )fileSizeInBytes,
            MultipartFormDataContent = multipartContent,
            PocketId = pocketId, // backend returns null if not provided
            FolderId = folderId.HasValue ? folderId : new Guid?() // backend returns null if not provided
        };

        return fileUnderTest;
    }
    
    private static MultipartFormDataContent CreateMultipartFormDataContent(
        double fileSizeInBytes,
        string fileName,
        string fileExtension,
        Guid? pocketId = null,
        Guid? folderId = null)
    {
        var fileBytes = new byte[(long)fileSizeInBytes];
        new Random().NextBytes(fileBytes);
        
        // 2. Wrap the byte array in a MemoryStream.
        var memoryStream = new MemoryStream(fileBytes);

        // 3. Create a StreamContent using the MemoryStream.
        var fileContent = new StreamContent(memoryStream);

        // 4. Set the appropriate headers for form-data.
        var fullyQualifiedFileName = $"{fileName}.{fileExtension}";
        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = $"\"{nameof(FileInformationModel.File)}\"",   // This must match the parameter name in <code>FileInformationModel</code>.
            FileName = $"\"{fullyQualifiedFileName}\"" // The file name can be arbitrary.
        };

        // Optional: Set the content type if your API expects it.
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        // 5. Create the MultipartFormDataContent container.
        var multipartContent = new MultipartFormDataContent();

        // 6. Add the file content to the multipart container.
        multipartContent.Add(fileContent, name: $"{nameof(FileInformationModel.File)}");
        
        // 7. Add the additional fields to the multipart container.

        if (pocketId.HasValue)
        {
            multipartContent.Add(new StringContent(pocketId.ToString()!),
                $"{nameof(FileInformationModel.PocketId)}");
        }

        if (folderId.HasValue)
        {
            multipartContent.Add(new StringContent(folderId.ToString()!),
                $"{nameof(FileInformationModel.FolderId)}");
        }

        return multipartContent;
    }

    internal static double GetFileSizeInBytes(this double fileSizeInMegabytes)
        => fileSizeInMegabytes * 1024f * 1024f;

    private static double GetFileSizeInBytes(this long fileSizeInMegabytes)
        => fileSizeInMegabytes * 1024f * 1024f;
}