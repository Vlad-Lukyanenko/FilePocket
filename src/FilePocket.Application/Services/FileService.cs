using AutoMapper;
using FilePocket.Contracts.Repositories;
using FilePocket.Contracts.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Entities.Consumption.Errors;
using FilePocket.Domain.Models;
using FilePocket.Shared.Exceptions;
using FilePocket.Shared.Extensions.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using StorageConsumption = FilePocket.Domain.Entities.Consumption.StorageConsumption;

namespace FilePocket.Application.Services;

public class FileService(
    IRepositoryManager repository,
    IConfiguration configuration,
    IImageService imageService,
    IMapper mapper) : IFileService
{
    private readonly string _rootFolder = configuration.GetValue<string>("AppRootFolder")!;

    public async Task<IEnumerable<FileResponseModel>> GetAllFilesAsync(Guid userId, Guid pocketId, Guid? folderId)
    {
        var fileMetadata = await repository.FileMetadata.GetAllAsync(userId, pocketId, folderId);

        var result = mapper.Map<List<FileResponseModel>>(fileMetadata);

        return result;
    }

    public async Task<FileResponseModel> GetFileByIdAsync(Guid userId, Guid fileId)
    {
        var fileMetadata = await GetFileByIdAndPocketIdAsync(userId, fileId);
        var fullPath = fileMetadata.GetFullPath();

        fullPath.EnsureFileExistsOnDisk();

        var fileByteArray = await File.ReadAllBytesAsync(fullPath);

        return new FileResponseModel
        {
            Id = fileMetadata.Id,
            DateCreated = fileMetadata.DateCreated,
            PocketId = fileMetadata.PocketId,
            FileSize = fileMetadata.FileSize,
            FileType = fileMetadata.FileType,
            FileByteArray = fileByteArray,
            OriginalName = fileMetadata.OriginalName
        };
    }

    public async Task<FileResponseModel> GetFileInfoByIdAsync(Guid userId, Guid fileId)
    {
        var fileMetadata = await repository.FileMetadata.GetByIdAsync(userId, fileId);

        return new FileResponseModel
        {
            Id = fileMetadata.Id,
            DateCreated = fileMetadata.DateCreated,
            PocketId = fileMetadata.PocketId,
            FileSize = fileMetadata.FileSize,
            FileType = fileMetadata.FileType,
            OriginalName = fileMetadata.OriginalName
        };
    }

    public async Task<List<FileResponseModel>> GetRecentFiles(Guid userId, int number)
    {
        var files = await repository.FileMetadata.GetRecentFilesAsync(userId, number);

        return mapper.Map<List<FileResponseModel>>(files);
    }

    public async Task<FileResponseModel> GetThumbnailAsync(Guid userId, Guid imageId, int maxSize)
    {
        return await GetThumbnailInternalAsync(userId, imageId, maxSize);
    }

    public async Task<List<FileResponseModel>> GetThumbnailsAsync(Guid userId, Guid[] imageIds, int maxSize)
    {
        var response = new List<FileResponseModel>();

        foreach (var imageId in imageIds)
        {
            var fileModel = await GetThumbnailInternalAsync(userId, imageId, maxSize);
            response.Add(fileModel);
        }

        return response;
    }

    public async Task<FileResponseModel?> UploadFileAsync(
        Guid userId,
        IFormFile file,
        Guid pocketId,
        Guid? folderId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        await using var fileUploadTransaction = await repository.BeginTransactionAsync(cancellationToken);
        try
        {
            var storageConsumption = await repository.AccountConsumption.GetStorageConsumptionAsync(
                userId, lockChanges: true, trackChanges: true, cancellationToken);

            if (storageConsumption is null)
                throw new AccountConsumptionNotFoundException(userId);

            var fileSizeInMbs = file.Length.ToMegabytes();
            if (storageConsumption.RemainingSizeMb < fileSizeInMbs)
                throw new InsufficientStorageCapacityException(
                    storageConsumption.Used,
                    storageConsumption.Total,
                    additionalUsedMb: fileSizeInMbs);

            var fileExtension = Path.GetExtension(file.FileName);
            var fileType = Tools.DefineFileType(fileExtension);
            var filePath = SelectFileDirectory(userId, pocketId, fileExtension);

            var fileMetadata = FileMetadata.Create(
                userId, file.FileName, filePath, fileType, fileSizeInMbs, pocketId, folderId);

            var fileMetadataTask = AttachFileToPocketTask(fileMetadata);

            var storageConsumptionTask = IncreaseStorageConsumptionTask(
                storageConsumption, fileMetadata);

            var fileUploadTask = UploadFileToDisk(fileMetadata);

            await Task.WhenAll(
                fileMetadataTask,
                storageConsumptionTask,
                fileUploadTask);

            await repository.SaveChangesAsync(cancellationToken);
            await fileUploadTransaction.CommitAsync(cancellationToken);

            return CreateFileResponseModel(fileMetadata);
        }
        catch (Exception)
        {
            await fileUploadTransaction.RollbackAsync(cancellationToken);
            throw;
        }

        async Task AttachFileToPocketTask(FileMetadata fileMetadata)
        {
            Pocket? pocket = null;

            pocket = await repository.Pocket.GetByIdAsync(userId, pocketId, trackChanges: true);

            if (pocket is null)
            {
                throw new PocketNotFoundException(pocketId);
            }

            pocket.UpdateDetails(fileMetadata);
            repository.FileMetadata.CreateFileMetadata(fileMetadata);
        }

        Task IncreaseStorageConsumptionTask(StorageConsumption storageConsumption, FileMetadata fileMetadata)
        {
            storageConsumption.IncreaseUsage(fileMetadata.FileSize);
            repository.AccountConsumption.Update(storageConsumption);
            return Task.CompletedTask;
        }

        async Task UploadFileToDisk(FileMetadata fileMetadata)
        {
            fileMetadata.Path.CreateFolderIfDoesNotExist();

            var fullPath = Path.Combine(fileMetadata.Path, fileMetadata.ActualName);

            fullPath.CheckIfFileNotExistsOnDisk();

            await using var fileStream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(fileStream, cancellationToken);
        }

        FileResponseModel? CreateFileResponseModel(FileMetadata? fileMetadata)
        {
            return fileMetadata is null ? null : new FileResponseModel
            {
                Id = fileMetadata.Id,
                UserId = fileMetadata.UserId,
                PocketId = fileMetadata.PocketId,
                FolderId = fileMetadata.FolderId,
                FileSize = fileMetadata.FileSize.ToBytes(),
                FileType = fileMetadata.FileType,
                ActualName = fileMetadata.ActualName,
                OriginalName = fileMetadata.OriginalName,
                DateCreated = fileMetadata.DateCreated,
            };
        }
    }

    public async Task<bool> RemoveFileAsync(
        Guid userId,
        Guid fileId,
        CancellationToken cancellationToken = default)
    {
        var fileMetadata = await repository.FileMetadata.GetByIdAsync(userId, fileId, trackChanges: true);
        if (fileMetadata is null)
            throw new FileMetadataNotFoundException(fileId);

        await using var transaction = await repository.BeginTransactionAsync(cancellationToken);
        try
        {
            var storageConsumption = await repository.AccountConsumption.GetStorageConsumptionAsync(
                userId, lockChanges: true, trackChanges: true, cancellationToken);

            if (storageConsumption is null)
            {
                throw new AccountConsumptionNotFoundException(userId);
            }
            
            RemoveFromFileSystemSync(fileMetadata);
            DecreaseStorageConsumption(storageConsumption, fileMetadata.FileSize);

            repository.FileMetadata.DeleteFileMetadata(fileMetadata);

            await repository.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        void DecreaseStorageConsumption(StorageConsumption storageConsumption, double fileSize)
        {
            storageConsumption.DecreaseUsage(fileSize);
            repository.AccountConsumption.Update(storageConsumption);
        }

        void RemoveFromFileSystemSync(FileMetadata fileToRemove)
        {
            var fullPath = Path.Combine(
                fileToRemove.Path,
                fileToRemove.ActualName);

            if (!File.Exists(fullPath))
                throw new FileDoesNotExistInFileSystemException(fileToRemove.Id);

            File.Delete(fullPath);
        }
    }

    private Task<FileMetadata> GetFileByIdAndPocketIdAsync(Guid userId, Guid fileId)
    {
        return repository.FileMetadata.GetByIdAsync(userId, fileId, true);
    }

    private string SelectFileDirectory(Guid userId, Guid? pocketId, string fileExtension)
    {
        var now = DateTime.UtcNow;

        var fileDirectory = string.Empty;

#if DEBUG
        fileDirectory = pocketId is not null
            ? $"{_rootFolder}\\{userId}\\{pocketId}\\{now.Year}\\{now.Month}\\{Tools.DefineFileType(fileExtension)}s"
            : $"{_rootFolder}\\{userId}\\{now.Year}\\{now.Month}\\{Tools.DefineFileType(fileExtension)}s";
#endif

#if !DEBUG
        fileDirectory = pocketId is not null 
            ? $"{_rootFolder}/{userId}/{pocketId}/{now.Year}/{now.Month}/{Tools.DefineFileType(fileExtension)}s"
            : $"{_rootFolder}/{userId}/{now.Year}/{now.Month}/{Tools.DefineFileType(fileExtension)}s";
#endif

        return fileDirectory;
    }

    private async Task<FileResponseModel> GetThumbnailInternalAsync(Guid userId, Guid id, int maxSize)
    {
        var fileMetadata = await GetFileByIdAndPocketIdAsync(userId, id);
        var fullPath = fileMetadata.GetFullPath();

        fullPath.EnsureFileExistsOnDisk();

        if (fileMetadata.FileType == FileTypes.Image)
        {
            fileMetadata.FileType!.CheckIfFileIsImage();

            var image = imageService.GetImage(fullPath);

            return GetResizedThumbnail(maxSize, fileMetadata, image.Width, image.Height, File.ReadAllBytes(fullPath));
        }
        if (fileMetadata.FileType == FileTypes.Video)
        {
            fileMetadata.FileType.CheckIfFileIsVideo();

            var frame = imageService.ExtractFirstFrame(fullPath);

            if (frame.FrameBytes.Length == 0)
            {
                return new FileResponseModel();
            }

            return GetResizedThumbnail(maxSize, fileMetadata, frame.Width, frame.Height, frame.FrameBytes);
        }
        return new FileResponseModel();
    }

    private FileResponseModel GetResizedThumbnail(int maxSize, FileMetadata fileMetadata, int width, int height, byte[] bytes)
    {
        int outWidth, outHeight;

        if (Math.Max(width, height) > maxSize)
        {
            if (width > height)
            {
                outWidth = maxSize;
                outHeight = (maxSize / width) * height;
            }
            else
            {
                outHeight = maxSize;
                outWidth = maxSize / height * width;
            }
        }
        else
        {
            outWidth = width;
            outHeight = height;
        }

        var thumbnailByteArray = imageService.ResizeImage(bytes, outWidth, outHeight);

        return new FileResponseModel
        {
            Id = fileMetadata.Id,
            FileByteArray = thumbnailByteArray,
            OriginalName = fileMetadata.OriginalName,
            FileType = fileMetadata.FileType,
            DateCreated = fileMetadata.DateCreated,
            PocketId = fileMetadata.PocketId,
            FileSize = fileMetadata.FileSize,
        };
    }
}
