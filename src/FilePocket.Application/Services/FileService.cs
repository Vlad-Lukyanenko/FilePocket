using AutoMapper;
using FilePocket.Contracts.Repositories;
using FilePocket.Contracts.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using FilePocket.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FilePocket.Application.Services;

public class FileService : IFileService
{
    private readonly string _rootFolder;
    private readonly IRepositoryManager _repository;
    private readonly IUploadService _uploadService;
    private readonly IImageService _imageService;
    private readonly IMapper _mapper;

    public FileService(
        IRepositoryManager repository,
        IConfiguration configuration,
        IUploadService uploadService,
        IImageService imageService,
        IMapper mapper)
    {
        _repository = repository;
        _rootFolder = configuration.GetValue<string>("AppRootFolder")!;
        _uploadService = uploadService;
        _imageService = imageService;
        _mapper = mapper;
    }

    public IUploadService UploadService { get => _uploadService; }

    public async Task<IEnumerable<FileResponseModel>> GetAllFilesAsync(Guid userId, Guid? pocketId, Guid? folderId)
    {
        var fileMetadata = await _repository.FileMetadata.GetAllAsync(userId, pocketId, folderId);

        var result = _mapper.Map<List<FileResponseModel>>(fileMetadata);

        return result;
    }

    public async Task<FileResponseModel> GetFileByIdAsync(Guid userId, Guid id)
    {
        var fileMetadata = await GetFileByIdAndPocketIdAsync(id);
        var fullPath = GetFullPath(fileMetadata);

        CheckIfFileExistsOnDisk(fullPath);

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
        var fileMetadata = await _repository.FileMetadata.GetByIdAsync(fileId);

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
        var files = await _repository.FileMetadata.GetRecentFilesAsync(userId, number);

        return _mapper.Map<List<FileResponseModel>>(files);
    }

    public async Task<FileResponseModel> GetThumbnailAsync(Guid userId, Guid imageId, int maxSize)
    {
        return await GetThumbnailInternalAsync(imageId, maxSize);
    }

    public async Task<List<FileResponseModel>> GetThumbnailsAsync(Guid userId, Guid[] imageIds, int maxSize)
    {
        var response = new List<FileResponseModel>();

        foreach (var imageId in imageIds)
        {
            var fileModel = await GetThumbnailInternalAsync(imageId, maxSize);
            response.Add(fileModel);
        }

        return response;
    }

    public async Task<FileResponseModel> UploadFileAsync(IFormFile file, Guid userId, Guid? pocketId, Guid? folderId)
    {
        var fileExtension = Path.GetExtension(file.FileName);
        var path = SelectFileDirectory(userId, pocketId, fileExtension);

        var fileMetadataToCreate = new FileMetadata
        {
            OriginalName = file.FileName,
            Path = path,
            FileType = Tools.DefineFileType(fileExtension),
            FileSize = file.Length / 1024,
            PocketId = pocketId,
            FolderId = folderId,
            UserId = userId,
        };

        _repository.FileMetadata.CreateFileMetadata(fileMetadataToCreate);

        if (pocketId is not null)
        {
            var pocket = await _repository.Pocket.GetByIdAsync(userId, pocketId.Value, true);
            AddFileToPocketEvent(pocket, fileMetadataToCreate.FileSize);
        }

        await _repository.SaveChangesAsync();

        CreateFolderIfDoesNotExist(path);

        var fullPath = Path.Combine(path, fileMetadataToCreate.ActualName);

        CheckIfFileNotExistsOnDisk(fullPath);

        await using (var fileStream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        };

        var result = new FileResponseModel
        {
            Id = fileMetadataToCreate.Id,
            OriginalName = file.FileName,
            FileType = fileMetadataToCreate.FileType,
            FileSize = file.Length / 1024,
            PocketId = pocketId,
            FolderId = folderId,
            UserId = userId,
        };

        return result;
    }

    public async Task DeleteFileAsync(Guid userId, Guid id)
    {
        var fileToDelete = await GetFileByIdAndPocketIdAsync(id);
        fileToDelete.IsDeleted = true;
        await _repository.SaveChangesAsync();
    }

    private Task<FileMetadata> GetFileByIdAndPocketIdAsync(Guid fileMetadataId)
    {
        return _repository.FileMetadata.GetByIdAsync(fileMetadataId, true);
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

    private static void CreateFolderIfDoesNotExist(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
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

    private static string GetFullPath(FileMetadata fileMetadata)
    {
        return fileMetadata.Path != null
            ? Path.Combine(fileMetadata.Path, fileMetadata.ActualName)
            : string.Empty;
    }

    private static void CheckIfFileIsImage(FileTypes? fileType)
    {
        if (fileType != FileTypes.Image)
        {
            throw new InvalidFileTypeException(fileType!.ToString());
        }
    }

    private static void CheckIfFileIsVideo(FileTypes? fileType)
    {
        if (fileType != FileTypes.Video)
        {
            throw new InvalidFileTypeException(fileType!.ToString());
        }
    }

    private async Task<FileResponseModel> GetThumbnailInternalAsync(Guid id, int maxSize)
    {
        var fileMetadata = await GetFileByIdAndPocketIdAsync(id);
        var fullPath = GetFullPath(fileMetadata);

        CheckIfFileExistsOnDisk(fullPath);

        if (fileMetadata.FileType == FileTypes.Image)
        {
            CheckIfFileIsImage(fileMetadata.FileType!);

            var image = _imageService.GetImage(fullPath);

            return GetResizedThumbnail(maxSize, fileMetadata, image.Width, image.Height, File.ReadAllBytes(fullPath));
        }
        if (fileMetadata.FileType == FileTypes.Video)
        {
            CheckIfFileIsVideo(fileMetadata.FileType!);

            var frame = _imageService.ExtractFirstFrame(fullPath);

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

        var thumbnailByteArray = _imageService.ResizeImage(bytes, outWidth, outHeight);

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

    private void AddFileToPocketEvent(Pocket pocket, double fileSize)
    {
        pocket.NumberOfFiles = pocket.NumberOfFiles is null ? 1 : pocket.NumberOfFiles + 1;
        pocket.TotalSize = pocket.TotalSize is null ? fileSize : pocket.TotalSize + fileSize;

        _repository.Pocket.UpdatePocket(pocket);
    }

    private void RemoveFileFromPocketEvent(Pocket pocket, double fileSize)
    {
        if (pocket.NumberOfFiles is not null && pocket.NumberOfFiles > 0)
        {
            pocket.NumberOfFiles -= 1;
        }

        if (pocket.TotalSize is not null && pocket.TotalSize >= fileSize)
        {
            pocket.TotalSize -= fileSize;
        }

        _repository.Pocket.UpdatePocket(pocket);
    }

}
