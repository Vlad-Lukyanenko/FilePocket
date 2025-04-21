using FilePocket.Application.Exceptions;
using FilePocket.Application.Extensions;
using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Entities.Consumption.Errors;
using FilePocket.Domain.Models;
using MapsterMapper;
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

    public async Task<IEnumerable<FileResponseModel>> GetAllFilesAsync(Guid userId, Guid pocketId, Guid? folderId, bool isSoftDeleted)
    {
        var fileMetadata = await repository.FileMetadata.GetAllAsync(userId, pocketId, folderId, isSoftDeleted);

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
            DateCreated = fileMetadata.CreatedAt,
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
            DateCreated = fileMetadata.CreatedAt,
            PocketId = fileMetadata.PocketId,
            FileSize = fileMetadata.FileSize,
            FileType = fileMetadata.FileType,
            OriginalName = fileMetadata.OriginalName
        };
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
                DateCreated = fileMetadata.CreatedAt,
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

    public async Task<FileResponseModel?> CreateNoteContentToFileAsync(Note note, byte[] content, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(note);

        await using var writeContentTransaction = await repository.BeginTransactionAsync(cancellationToken);
        try
        {
            var storageConsumption = await repository.AccountConsumption.GetStorageConsumptionAsync(
                note.UserId, lockChanges: true, trackChanges: true, cancellationToken);

            if (storageConsumption is null)
                throw new AccountConsumptionNotFoundException(note.UserId);

            var fileSizeInMbs = ((long)content.Length).ToMegabytes();
            if (storageConsumption.RemainingSizeMb < fileSizeInMbs)
                throw new InsufficientStorageCapacityException(
                    storageConsumption.Used,
                    storageConsumption.Total,
                    additionalUsedMb: fileSizeInMbs);

            var fileExtension = GetNoteFileExtension();
            var fileType = Tools.DefineFileType(fileExtension);
            var filePath = SelectFileDirectory(note.UserId, note.PocketId, fileExtension);
            var fileName = GetNoteFileNameWithExtension(note.Id);

            var fileMetadata = FileMetadata.Create(
                note.UserId, fileName, filePath, fileType, fileSizeInMbs, note.PocketId, note.FolderId);

            var contentFileMetadataTask = AttachFileToPocketTask(fileMetadata, note.UserId, note.PocketId);
            var storageConsumptionTask = ChangeStorageConsumption(storageConsumption, fileMetadata.FileSize);
            var WriteContentFileTask = WriteContentToFile(fileMetadata);

            await Task.WhenAll(
                contentFileMetadataTask,
                storageConsumptionTask,
                WriteContentFileTask);

            await repository.SaveChangesAsync(cancellationToken);
            await writeContentTransaction.CommitAsync(cancellationToken);

            return GetFileResponseModel(fileMetadata);
        }
        catch (Exception)
        {
            await writeContentTransaction.RollbackAsync(cancellationToken);
            throw;
        }


        async Task WriteContentToFile(FileMetadata fileMetadata)
        {
            fileMetadata.Path.CreateFolderIfDoesNotExist();

            var fullPath = Path.Combine(fileMetadata.Path, fileMetadata.ActualName);

            fullPath.CheckIfFileNotExistsOnDisk();

            await File.WriteAllBytesAsync(fullPath, content, cancellationToken);
        }
    }

    public async Task<FileResponseModel?> UpdateNoteContentFileAsync(Note note, byte[] content, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(note);
        var fileMetadata = await repository.FileMetadata.GetByIdAsync(note.UserId, note.ContentFileMetadataId, trackChanges: true);
        if (fileMetadata is null)
            throw new FileMetadataNotFoundException(note.ContentFileMetadataId);

        await using var transaction = await repository.BeginTransactionAsync(cancellationToken);
        try
        {
            var storageConsumption = await repository.AccountConsumption.GetStorageConsumptionAsync(
                note.UserId, lockChanges: true, trackChanges: true, cancellationToken);

            if (storageConsumption is null)
            {
                throw new AccountConsumptionNotFoundException(note.UserId);
            }

            var newFileSizeInMbs = ((long)content.Length).ToMegabytes();
            var consumptionChangeInMbs = newFileSizeInMbs - ((long)fileMetadata.FileSize).ToMegabytes();
            if (storageConsumption.RemainingSizeMb < consumptionChangeInMbs)
            {
                throw new InsufficientStorageCapacityException(
                    storageConsumption.Used,
                    storageConsumption.Total,
                    additionalUsedMb: consumptionChangeInMbs);
            }

            var fileExtension = GetNoteFileExtension();
            var fileType = Tools.DefineFileType(fileExtension);
            var filePath = SelectFileDirectory(note.UserId, note.PocketId, fileExtension);
            var fileName = GetNoteFileNameWithExtension(note.Id);

            var updatedFileMetadata = FileMetadata.Create(
                note.UserId, fileName, filePath, fileType, newFileSizeInMbs, note.PocketId, note.FolderId);

            var contentFileMetadataTask = AttachFileToPocketTask(updatedFileMetadata, note.UserId, note.PocketId);
            var storageConsumptionTask = ChangeStorageConsumption(storageConsumption, consumptionChangeInMbs);
            var rewriteFileTask = RewriteToFileSystemAsync(updatedFileMetadata);

            await Task.WhenAll(
                contentFileMetadataTask,
                storageConsumptionTask,
                rewriteFileTask);

            RemoveFromFileSystem(fileMetadata);
            repository.FileMetadata.DeleteFileMetadata(fileMetadata);

            await repository.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return GetFileResponseModel(updatedFileMetadata);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        void RemoveFromFileSystem(FileMetadata file)
        {
            var fullPath = Path.Combine(
                file.Path,
                file.ActualName);

            if (!File.Exists(fullPath))
                throw new FileDoesNotExistInFileSystemException(file.Id);

            File.Delete(fullPath);
        }

        async Task RewriteToFileSystemAsync(FileMetadata file)
        {
            file.Path.CreateFolderIfDoesNotExist();
            var fullPath = Path.Combine(file.Path, file.ActualName);
            await File.WriteAllBytesAsync(fullPath, content, cancellationToken);
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
            DateCreated = fileMetadata.CreatedAt,
            PocketId = fileMetadata.PocketId,
            FileSize = fileMetadata.FileSize,
        };
    }

    private async Task AttachFileToPocketTask(FileMetadata fileMetadata, Guid userId, Guid pocketId)
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

    private FileResponseModel? GetFileResponseModel(FileMetadata? fileMetadata)
    {
        return fileMetadata is null ? null : new FileResponseModel
        {
            Id = fileMetadata.Id
        };
    }

    private string GetNoteFileExtension()
    {
        return configuration.GetValue<string>("NoteContentFileExtension")!;
    }

    private string GetNoteFileNameWithExtension(Guid noteId)
    {
        var fileExtension = GetNoteFileExtension();
        return $"{noteId}{fileExtension}";
    }
}
