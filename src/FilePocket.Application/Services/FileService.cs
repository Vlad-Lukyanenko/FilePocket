using FilePocket.Application.Exceptions;
using FilePocket.Application.Extensions;
using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Entities.Consumption.Errors;
using FilePocket.Domain.Models;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using StorageConsumption = FilePocket.Domain.Entities.Consumption.StorageConsumption;

namespace FilePocket.Application.Services;

public class FileService(
    IRepositoryManager repository,
    IConfiguration configuration,
    IImageService imageService,
    IEncryptionService encryptionService,
    IMapper mapper) : IFileService
{
    private readonly string _rootFolder = configuration.GetValue<string>("AppRootFolder")!;
    private readonly string _noteFileExtension = configuration.GetValue<string>("NoteContentFileExtension")!;

    #region Implementation of IFileService

    public async Task<IEnumerable<FileResponseModel>> GetAllFilesMetadataAsync(Guid userId, Guid pocketId, Guid? folderId, bool isSoftDeleted)
    {
        var fileMetadata = await repository.FileMetadata.GetAllAsync(userId, pocketId, folderId, isSoftDeleted);

        var result = mapper.Map<List<FileResponseModel>>(fileMetadata);

        return result;
    }
    
    public async Task<IEnumerable<FileResponseModel>> GetAllFilesWithSoftDeletedAsync(Guid userId, Guid pocketId)
    {
        var fileMetadata = await repository.FileMetadata.GetAllWithSoftDeletedAsync(userId, pocketId);

        var result = mapper.Map<List<FileResponseModel>>(fileMetadata);

        return result;
    }

    public async Task<IEnumerable<NoteModel>> GetAllNotesMetadataAsync(Guid userId, Guid? folderId, bool isSoftDeleted)
    {
        var fileMetadata = await repository.FileMetadata.GetAllNotesAsync(userId, folderId, isSoftDeleted);
        var result = mapper.Map<List<NoteModel>>(fileMetadata);

        return result;
    }

    public async Task<FileResponseModel> GetFileByUserIdIdAsync(Guid userId, Guid fileId)
    {
        var fileMetadata = await repository.FileMetadata.GetByIdAsync(userId, fileId, true);

        var fullPath = fileMetadata.GetFullPath();

        fullPath.EnsureFileExistsOnDisk();

        var fileByteArray = await File.ReadAllBytesAsync(fullPath) ?? [];

        return new FileResponseModel
        {
            Id = fileMetadata.Id,
            CreatedAt = fileMetadata.CreatedAt,
            UpdatedAt = fileMetadata.UpdatedAt,
            PocketId = fileMetadata.PocketId,
            FileSize = fileMetadata.FileSize,
            FileType = fileMetadata.FileType,
            FileByteArray = fileByteArray,
            OriginalName = fileMetadata.OriginalName
        };
    }

    public async Task<FileResponseModel> GetFileMetadataByUserIdAndIdAsync(Guid userId, Guid fileId)
    {
        var fileMetadata = await repository.FileMetadata.GetByIdAsync(userId, fileId);

        return new FileResponseModel
        {
            Id = fileMetadata.Id,
            CreatedAt = fileMetadata.CreatedAt,
            UpdatedAt = fileMetadata.UpdatedAt,
            PocketId = fileMetadata.PocketId,
            FolderId = fileMetadata.FolderId,
            FileSize = fileMetadata.FileSize,
            FileType = fileMetadata.FileType,
            OriginalName = fileMetadata.OriginalName,
            UserId = fileMetadata.UserId,
        };
    }

    public async Task<NoteModel> GetNoteByUserIdAndIdAsync(Guid userId, Guid fileId)
    {
        var fileMetadata = await GetFileMetadataByUserIdAndIdAsync(userId, fileId);
        var note = mapper.Map<NoteModel>(fileMetadata);

        fileMetadata.FileByteArray = await ReadNoteContentFromFileAsync(userId, fileId);
        note.Content = await encryptionService.DecryptContent(note.UserId, note.PocketId, fileMetadata.FileByteArray!);

        return note;
    }

    public async Task<List<FileResponseModel>> GetLatestAsync(Guid userId, int number)
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
            var filePath = SelectFileDirectory(userId, pocketId, fileType);

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
                CreatedAt = fileMetadata.CreatedAt,
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

    public async Task<bool> MoveToTrash(Guid userId, Guid fileId, CancellationToken cancellationToken = default)
    {
        var fileMetadata = await repository.FileMetadata.GetByIdAsync(userId, fileId, trackChanges: true);
        if (fileMetadata is null)
        {
            throw new FileMetadataNotFoundException(fileId);
        }

        fileMetadata.MarkAsDeleted();

        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task UpdateFileAsync(UpdateFileModel file)
    {
        var fileMetadataToUpdate = await repository.FileMetadata.GetByIdAsync(file.UserId, file.Id, trackChanges: true);

        if (fileMetadataToUpdate is null)
        {
            throw new FileMetadataNotFoundException(file.Id);
        }

        mapper.Map(file, fileMetadataToUpdate);

        await repository.SaveChangesAsync();
    }

    public async Task<FileResponseModel?> CreateNoteContentFileAsync(NoteCreateModel note, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(note);

        var contentBytes = await encryptionService.EncryptContent(note.UserId, note.PocketId, note.Content, cancellationToken);

        await using var writeContentTransaction = await repository.BeginTransactionAsync(cancellationToken);
        try
        {
            var storageConsumption = await repository.AccountConsumption.GetStorageConsumptionAsync(
                note.UserId,
                lockChanges: true,
                trackChanges: true,
                cancellationToken) ?? throw new AccountConsumptionNotFoundException(note.UserId);

            var fileSizeInMbs = ((long)contentBytes.Length).ToMegabytes();

            if (storageConsumption.RemainingSizeMb < fileSizeInMbs)
                throw new InsufficientStorageCapacityException(
                    storageConsumption.Used,
                    storageConsumption.Total,
                    additionalUsedMb: fileSizeInMbs);

            var fileType = Tools.DefineFileType(_noteFileExtension);
            var filePath = SelectFileDirectory(note.UserId, note.PocketId, fileType);

            var fileMetadata = FileMetadata.Create(
                note.UserId, note.Title, filePath, fileType, fileSizeInMbs, note.PocketId, note.FolderId);

            var pocketAndMetadataTask = UpdateTargetPocketAndMetadata(fileMetadata);
            var storageConsumptionTask = ChangeStorageConsumption(storageConsumption, fileMetadata.FileSize);
            var writeContentToFileTask = WriteContentToFile(fileMetadata, contentBytes, WriteFileMode.Create, cancellationToken);

            await Task.WhenAll(
                pocketAndMetadataTask,
                storageConsumptionTask,
                writeContentToFileTask);

            await repository.SaveChangesAsync(cancellationToken);
            await writeContentTransaction.CommitAsync(cancellationToken);

            return GetFileResponseModel(fileMetadata, FileResponseModelType.Create);
        }
        catch (Exception)
        {
            await writeContentTransaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<FileResponseModel?> UpdateNoteContentFileAsync(NoteModel note, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(note);

        var fileMetadata = await repository.FileMetadata.GetByIdAsync(note.UserId, note.Id, trackChanges: true)
            ?? throw new FileMetadataNotFoundException(note.Id);

        var contentBytes = await encryptionService.EncryptContent(note.UserId, note.PocketId, note.Content, cancellationToken);

        await using var transaction = await repository.BeginTransactionAsync(cancellationToken);
        try
        {
            var storageConsumption = await repository.AccountConsumption.GetStorageConsumptionAsync(
                note.UserId,
                lockChanges: true,
                trackChanges: true,
                cancellationToken) ?? throw new AccountConsumptionNotFoundException(note.UserId);

            var newFileSizeInMbs = ((long)contentBytes.Length).ToMegabytes();
            var sizeChangeInMbs = newFileSizeInMbs - fileMetadata.FileSize;

            if (storageConsumption.RemainingSizeMb < sizeChangeInMbs)
            {
                throw new InsufficientStorageCapacityException(
                    storageConsumption.Used,
                    storageConsumption.Total,
                    additionalUsedMb: sizeChangeInMbs);
            }

            var fileType = Tools.DefineFileType(_noteFileExtension);
            var filePath = SelectFileDirectory(note.UserId, note.PocketId, fileType);

            fileMetadata.OriginalName = note.Title;
            fileMetadata.Path = filePath;
            fileMetadata.FileSize = newFileSizeInMbs;
            fileMetadata.PocketId = note.PocketId;
            fileMetadata.FolderId = note.FolderId;

            var pocketAndMetadataTask = UpdateTargetPocketAndMetadata(fileMetadata, sizeChangeInMbs);
            var storageConsumptionTask = ChangeStorageConsumption(storageConsumption, sizeChangeInMbs);
            var writeContentToFileTask = WriteContentToFile(fileMetadata, contentBytes, WriteFileMode.Override, cancellationToken);

            await Task.WhenAll(
                pocketAndMetadataTask,
                storageConsumptionTask,
                writeContentToFileTask);

            await repository.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return GetFileResponseModel(fileMetadata, FileResponseModelType.Update);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<byte[]> ReadNoteContentFromFileAsync(Guid userId, Guid fileId)
    {
        var fileMetadata = await repository.FileMetadata.GetByIdAsync(userId, fileId, trackChanges: true)
            ?? throw new FileMetadataNotFoundException(fileId);

        var fullPath = Path.Combine(fileMetadata.Path, fileMetadata.ActualName);

        fullPath.EnsureFileExistsOnDisk();

        return await File.ReadAllBytesAsync(fullPath);
    }

    public async Task<IEnumerable<FileSearchResponseModel>> SearchAsync(Guid userId, string partialName)
    {
        var files = await repository.FileMetadata.GetFileMetadataByPartialNameAsync(userId, partialName);

        return mapper.Map<IEnumerable<FileSearchResponseModel>>(files);
    }

    #endregion

    #region Private Methods

    private string SelectFileDirectory(Guid userId, Guid? pocketId, FileTypes fileType)
    {
        var now = DateTime.UtcNow;

        var fileDirectory = string.Empty;

#if DEBUG
        fileDirectory = pocketId is not null
            ? $"{_rootFolder}\\{userId}\\{pocketId}\\{now.Year}\\{now.Month}\\{fileType}s"
            : $"{_rootFolder}\\{userId}\\{now.Year}\\{now.Month}\\{fileType}s";
#endif

#if !DEBUG
        fileDirectory = pocketId is not null 
            ? $"{_rootFolder}/{userId}/{pocketId}/{now.Year}/{now.Month}/{fileType}s"
            : $"{_rootFolder}/{userId}/{now.Year}/{now.Month}/{fileType}s";
#endif

        return fileDirectory;
    }

    private async Task<FileResponseModel> GetThumbnailInternalAsync(Guid userId, Guid id, int maxSize)
    {
        var fileMetadata = await repository.FileMetadata.GetByIdAsync(userId, id, true);
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
            CreatedAt = fileMetadata.CreatedAt,
            PocketId = fileMetadata.PocketId,
            FileSize = fileMetadata.FileSize,
        };
    }

    private async Task UpdateTargetPocketAndMetadata(FileMetadata fileMetadata, double sizeChange = 0)
    {
        var pocket = await repository.Pocket.GetByIdAsync(fileMetadata.UserId, fileMetadata.PocketId, trackChanges: true)
            ?? throw new PocketNotFoundException(fileMetadata.PocketId);

        if (sizeChange == 0)
        {
            pocket.UpdateDetails(fileMetadata);
            repository.FileMetadata.CreateFileMetadata(fileMetadata);
        }
        else
        {
            pocket.UpdateDetails(sizeChange);
            fileMetadata.UpdatedAt = DateTime.UtcNow;
            repository.FileMetadata.UpdateFileMetadata(fileMetadata);
        }
    }

    private Task ChangeStorageConsumption(StorageConsumption storageConsumption, double consumptionChangeValue)
    {
        if (consumptionChangeValue > 0)
        {
            storageConsumption.IncreaseUsage(consumptionChangeValue);
        }
        else if (consumptionChangeValue < 0)
        {
            storageConsumption.DecreaseUsage(Math.Abs(consumptionChangeValue));
        }

        repository.AccountConsumption.Update(storageConsumption);

        return Task.CompletedTask;
    }

    private static FileResponseModel? GetFileResponseModel(FileMetadata? fileMetadata, FileResponseModelType responseType)
    {
        if (fileMetadata is null) return null;

        if (responseType == FileResponseModelType.Create)
            return new FileResponseModel
            {
                Id = fileMetadata.Id,
                CreatedAt = fileMetadata.CreatedAt,
            };

        if (responseType == FileResponseModelType.Update)
        {
            var response = new FileResponseModel
            {
                UpdatedAt = fileMetadata.UpdatedAt!.Value,
            };

            return response;
        }

        return new FileResponseModel
        {
            Id = fileMetadata.Id,
            UserId = fileMetadata.UserId,
            PocketId = fileMetadata.PocketId,
            FolderId = fileMetadata.FolderId,
            FileSize = fileMetadata.FileSize.ToBytes(),
            FileType = fileMetadata.FileType,
            ActualName = fileMetadata.ActualName,
            OriginalName = fileMetadata.OriginalName,
            CreatedAt = fileMetadata.CreatedAt,
        };
    }

    private static async Task WriteContentToFile(FileMetadata fileMetadata, byte[] content, WriteFileMode mode, CancellationToken cancellationToken)
    {
        fileMetadata.Path.CreateFolderIfDoesNotExist();

        var fullPath = Path.Combine(fileMetadata.Path, fileMetadata.ActualName);

        if (mode == WriteFileMode.Create)
        {
            fullPath.CheckIfFileNotExistsOnDisk();
        }

        await File.WriteAllBytesAsync(fullPath, content, cancellationToken);
    }

    #endregion

    private enum WriteFileMode
    {
        Create,
        Override
    }

    private enum FileResponseModelType
    {
        Upload,
        Create,
        Update
    }
}
