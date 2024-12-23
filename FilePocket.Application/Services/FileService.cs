﻿using FilePocket.Contracts.Repositories;
using FilePocket.Contracts.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using FilePocket.Shared.Exceptions;
using MagpieChat.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;


namespace FilePocket.Application.Services;

public class FileService : IFileService
{
    private readonly string _rootFolder;
    private readonly IRepositoryManager _repository;
    private readonly IUploadService _uploadService;
    private readonly IImageService _imageService;

    public FileService(IRepositoryManager repository, IConfiguration configuration, IUploadService uploadService, IImageService imageService)
    {
        _repository = repository;
        _rootFolder = configuration.GetValue<string>("AppRootFolder")!;
        _uploadService = uploadService;
        _imageService = imageService;
    }

    public IUploadService UploadService { get => _uploadService; }

    public async Task<IEnumerable<FileResponseModel>> GetAllFilesByStorageIdAsync(Guid storageId)
    {
        var storage = await _repository.Storage.GetByIdAsync(storageId);

        CheckIfStorageExists(storage, storageId);

        var fileUploadSummaries = await _repository.FileUploadSummary.GetAllByStorageIdAsync(storageId);
        var filesResponses = new List<FileResponseModel>();

        foreach (var fileSummary in fileUploadSummaries)
        {
            var fullPath = GetFullPath(fileSummary);

            CheckIfFileExistsOnDisk(fullPath);

            var fileByteArray = await File.ReadAllBytesAsync(fullPath);
            var storageName = (await _repository.Storage.GetByIdAsync(storageId)).Name;

            filesResponses.Add(new FileResponseModel
            {
                Id = fileSummary.Id,
                FileByteArray = fileByteArray,
                OriginalName = fileSummary.OriginalName,
                FileType = fileSummary.FileType,
                DateCreated = fileSummary.DateCreated,
                StorageId = fileSummary.StorageId,
                StorageName = storageName
            });
        }

        return filesResponses;
    }

    public async Task<IEnumerable<FileResponseModel>> GetFilteredFilesAsync(FilesFilterOptionsModel filterOptionsModel)
    {
        var filesResponses = new List<FileResponseModel>();

        var fileUploadSummaries = await _repository.FileUploadSummary.GetFilteredFilesAsync(filterOptionsModel);
        var usersStorages = (await _repository.Storage.GetAllByUserIdAsync(filterOptionsModel.UserId)).ToList();

        foreach (var fileSummary in fileUploadSummaries)
        {
            var storageName = usersStorages.FirstOrDefault(s => s.Id == fileSummary.StorageId)?.Name;

            if (storageName is null)
            {
                continue;
            }

            filesResponses.Add(new FileResponseModel
            {
                Id = fileSummary.Id,
                OriginalName = fileSummary.OriginalName,
                FileType = fileSummary.FileType,
                DateCreated = fileSummary.DateCreated,
                StorageId = fileSummary.StorageId,
                FileSize = fileSummary.FileSize,
                StorageName = storageName,
            });
        }

        return filesResponses;
    }

    public async Task<int> GetFilteredFilesCountAsync(FilesFilterOptionsModel filterOptionsModel)
    {
        return await _repository.FileUploadSummary.GetFilteredCountAsync(filterOptionsModel);
    }

    public async Task<FileResponseModel> GetFileByIdAsync(Guid storageId, Guid id)
    {
        var storage = await _repository.Storage.GetByIdAsync(storageId);

        CheckIfStorageExists(storage, storageId);

        var fileUploadSummary = await GetFileByIdAndStorageIdAsync(storageId, id);
        var fullPath = GetFullPath(fileUploadSummary);

        CheckIfFileExistsInStorage(fileUploadSummary, id);
        CheckIfFileExistsOnDisk(fullPath);

        var fileByteArray = await File.ReadAllBytesAsync(fullPath);

        return new FileResponseModel { FileByteArray = fileByteArray, OriginalName = fileUploadSummary.OriginalName };
    }

    public async Task<FileResponseModel> GetThumbnailAsync(Guid storageId, Guid id, int maxSize)
    {
        var storage = await _repository.Storage.GetByIdAsync(storageId);

        CheckIfStorageExists(storage, storageId);

        return await GetThumbnailInternalAsync(id, maxSize, storage);
    }

    public async Task<List<FileResponseModel>> GetThumbnailsAsync(List<UserIconInfoRequest> request, Guid storageId, int maxSize)
    {
        var storage = await _repository.Storage.GetByIdAsync(storageId);

        CheckIfStorageExists(storage, storageId);

        var response = new List<FileResponseModel>();

        foreach (var item in request)
        {
            var fileModel = await GetThumbnailInternalAsync(item.IconId, maxSize, storage);
            response.Add(fileModel);
        }

        return response;
    }

    public async Task<FileUploadSummary> UploadFileAsync(IFormFile file, Guid userId, Guid storageId)
    {
        var storage = await _repository.Storage.GetByIdAsync(storageId);

        CheckIfStorageExists(storage, storageId);

        var fileExtension = Path.GetExtension(file.FileName);
        var path = SelectFileDirectory(userId, storageId, fileExtension);

        var fileUploadSummaryToCreate = new FileUploadSummary
        {
            OriginalName = file.FileName,
            Path = path,
            FileType = DefineFileType(fileExtension),
            FileSize = file.Length / 1024,
            StorageId = storageId
        };

        _repository.FileUploadSummary.CreateFileUploadSummary(fileUploadSummaryToCreate);
        await _repository.SaveChangesAsync();

        CreateFolderIfDoesNotExist(path);

        var fullPath = Path.Combine(path, fileUploadSummaryToCreate.ActualName);

        CheckIfFileNotExistsOnDisk(fullPath);

        await using (var fileStream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        };

        return fileUploadSummaryToCreate;
    }

    public Session CreateSession(Guid userId, Guid storageId, CreateSessionParams sessionParams)
    {
        var session = UploadService.CreateSession(
            userId,
            storageId,
            sessionParams.FileName!,
            sessionParams.FileSize!.Value);

        session.ChunksDirectory = SelectFileChunksDirectory(userId, session.Id);

        CreateFolderIfDoesNotExist(session.ChunksDirectory);

        return session;
    }

    public async Task UploadFileChunkAsync(IFormFile file, string userId, string storageId, string sessionId, int chunkNumber)
    {
        await _uploadService.PersistBlock(sessionId, chunkNumber, ToByteArray(file.OpenReadStream()));

        var session = _uploadService.GetSession(sessionId);

        if (session.IsConcluded)
        {
            await FinalizeUploding(file, Guid.Parse(userId), Guid.Parse(storageId), session);
        }
    }

    public async Task DeleteFileAsync(Guid storageId, Guid id)
    {
        var storage = await _repository.Storage.GetByIdAsync(storageId);

        CheckIfStorageExists(storage, storageId);

        var fileUploadSummaryToDelete = await GetFileByIdAndStorageIdAsync(storageId, id);
        var fullPath = GetFullPath(fileUploadSummaryToDelete);

        CheckIfFileExistsInStorage(fileUploadSummaryToDelete, id);
        CheckIfFileExistsOnDisk(fullPath);

        File.Delete(fullPath);

        _repository.FileUploadSummary.DeleteFileUploadSummary(fileUploadSummaryToDelete);
        await _repository.SaveChangesAsync();
    }

    private async Task<FileUploadSummary> GetFileByIdAndStorageIdAsync(Guid storageId, Guid fileUploadSummaryId)
    {
        var fileUploadSummary = await _repository.FileUploadSummary.GetByIdAsync(storageId, fileUploadSummaryId);

        CheckIfFileExistsInStorage(fileUploadSummary, fileUploadSummaryId);

        return fileUploadSummary;
    }

    public async Task<bool> CheckIfFileExists(string fileName, Guid storageId)
    {
        var fileExtension = Path.GetExtension(fileName);
        var fileType = DefineFileType(fileExtension);
        return await _repository.FileUploadSummary.CheckIfFileExists(fileName, fileType, storageId);
    }

    private string SelectFileDirectory(Guid userId, Guid storageId, string fileExtension)
    {
        var now = DateTime.UtcNow;

        var fileDirectory = string.Empty;

#if DEBUG
        fileDirectory = $"{_rootFolder}\\{userId}\\{storageId}\\{now.Year}\\{now.Month}\\{DefineFileType(fileExtension)}s";
#endif

#if !DEBUG
        fileDirectory = $"{_rootFolder}/{userId}/{storageId}/{now.Year}/{now.Month}/{DefineFileType(fileExtension)}s";
#endif

        return fileDirectory;
    }

    private string SelectFileChunksDirectory(Guid userId, Guid sessionId)
    {
        var filePath = string.Empty;

#if DEBUG
        filePath = $"{_rootFolder}\\{userId}\\UploadSessions\\{sessionId}";
#endif

#if !DEBUG
        filePath = $"{_rootFolder}/{userId}/UploadSessions/{sessionId}";
#endif

        return filePath;
    }

    private static string DefineFileType(string fileExtension)
    {
        FileTypes fileType;

        switch (fileExtension.ToLower())
        {
            case ".pdf":
            case ".doc":
            case ".docx":
            case ".txt":
            case ".rtf":
                fileType = FileTypes.Document;
                break;

            case ".png":
            case ".jpeg":
            case ".jpg":
                fileType = FileTypes.Image;
                break;

            case ".mp3":
            case ".wav":
            case ".vma":
                fileType = FileTypes.Audio;
                break;

            case ".mp4":
            case ".mov":
            case ".wmv":
            case ".avi":
                fileType = FileTypes.Video;
                break;

            default:
                fileType = FileTypes.Other;
                break;
        }

        return fileType.ToString();
    }

    private static void CreateFolderIfDoesNotExist(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    private static byte[] ToByteArray(Stream stream)
    {
        using (var ms = new MemoryStream())
        {
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }

    private static void CheckIfStorageExists(Storage? storage, Guid storageId)
    {
        if (storage is null)
        {
            throw new StorageNotFoundException(storageId);
        }
    }

    private static void CheckIfFileExistsOnDisk(string fullPath)
    {

        if (!File.Exists(fullPath))
        {
            throw new FileOnLocalMachineNotFoundException(Path.GetDirectoryName(fullPath)!);
        }
    }

    private static void CheckIfFileNotExistsOnDisk(string fullPath)
    {
        if (File.Exists(fullPath))
        {
            throw new FileAlreadyUploadedException(Path.GetDirectoryName(fullPath)!);
        }
    }

    private static string GetFullPath(FileUploadSummary fileUploadSummary)
    {
        return fileUploadSummary.Path != null
            ? Path.Combine(fileUploadSummary.Path, fileUploadSummary.ActualName)
            : string.Empty;
    }

    private static void CheckIfFileExistsInStorage(FileUploadSummary? fileUploadSummary, Guid fileId)
    {
        if (fileUploadSummary is null)
        {
            throw new FileUploadSummaryNotFoundException(fileId);
        }
    }

    private static void CheckIfFileIsImage(string fileType)
    {
        if (fileType != FileTypes.Image.ToString())
        {
            throw new InvalidFileTypeException(fileType);
        }
    }

    private static void CheckIfFileIsVideo(string fileType)
    {
        if (fileType != FileTypes.Video.ToString())
        {
            throw new InvalidFileTypeException(fileType);
        }
    }

    private async Task FinalizeUploding(IFormFile file, Guid userId, Guid storageId, Session session)
    {
        var storage = await _repository.Storage.GetByIdAsync(storageId);

        CheckIfStorageExists(storage, storageId);

        var fileExtension = Path.GetExtension(session.FileInfo.OriginalName);
        var path = SelectFileDirectory(userId, storageId, fileExtension);

        CreateFolderIfDoesNotExist(path);

        var fullPath = Path.Combine(path, session.FileInfo.OriginalName);

        if (File.Exists(fullPath))
        {
            throw new FileAlreadyUploadedException(session.FileInfo.OriginalName);
        }

        string[] chunks = Directory.GetFiles(session.ChunksDirectory);

        await using (var outputStream = new FileStream(fullPath, FileMode.Create))
        {
            foreach (var chunk in chunks)
            {
                await using var inputStream = new FileStream(chunk, FileMode.Open);
                await inputStream.CopyToAsync(outputStream);
            }
        };

        var fileSizeKb = new FileInfo(fullPath).Length / 1024.0;

        var fileUploadSummaryToCreate = new FileUploadSummary
        {
            OriginalName = file.FileName,
            Path = fullPath,
            FileType = DefineFileType(fileExtension),
            FileSize = fileSizeKb,
            StorageId = storageId
        };

        _repository.FileUploadSummary.CreateFileUploadSummary(fileUploadSummaryToCreate);

        await _repository.SaveChangesAsync();

        _uploadService.DeleteSession(session.Id.ToString());

        Directory.Delete(session.ChunksDirectory, true);
    }

    private async Task<FileResponseModel> GetThumbnailInternalAsync(Guid id, int maxSize, Storage storage)
    {
        var fileUploadSummary = await GetFileByIdAndStorageIdAsync(storage.Id, id);
        var fullPath = GetFullPath(fileUploadSummary);

        CheckIfFileExistsInStorage(fileUploadSummary, id);
        CheckIfFileExistsOnDisk(fullPath);

        if (fileUploadSummary.FileType == FileTypes.Image.ToString())
        {
            CheckIfFileIsImage(fileUploadSummary.FileType!);

            var image = _imageService.GetImage(fullPath);

            return GetResizedThumbnail(maxSize, storage.Name!, fileUploadSummary, image.Width, image.Height, File.ReadAllBytes(fullPath));
        }
        if (fileUploadSummary.FileType == FileTypes.Video.ToString())
        {
            CheckIfFileIsVideo(fileUploadSummary.FileType!);

            var frame = _imageService.ExtractFirstFrame(fullPath);

            if (frame.FrameBytes.Length == 0)
            {
                return new FileResponseModel();
            }

            return GetResizedThumbnail(maxSize, storage.Name!, fileUploadSummary, frame.Width, frame.Height, frame.FrameBytes);
        }
        return new FileResponseModel();
    }

    private FileResponseModel GetResizedThumbnail(int maxSize, string storageName, FileUploadSummary fileUploadSummary, int width, int height, byte[] bytes)
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

        var thumbnailByteArray = _imageService.ResizeImage(bytes, outWidth, outHeight);

        return new FileResponseModel
        {
            Id = fileUploadSummary.Id,
            FileByteArray = thumbnailByteArray,
            OriginalName = fileUploadSummary.OriginalName,
            FileType = fileUploadSummary.FileType,
            DateCreated = fileUploadSummary.DateCreated,
            StorageId = fileUploadSummary.StorageId,
            FileSize = fileUploadSummary.FileSize,
            StorageName = storageName
        };
    }
}
