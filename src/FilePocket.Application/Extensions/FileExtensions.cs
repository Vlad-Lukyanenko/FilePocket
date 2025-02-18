using FilePocket.Application.Exceptions;
using FilePocket.Domain;
using FilePocket.Domain.Entities;

namespace FilePocket.Application.Extensions;

public static class FileExtensions
{
    public static void CreateFolderIfDoesNotExist(this string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    public static void EnsureFileExistsOnDisk(this string fullPath)
    {
        if (!File.Exists(fullPath))
        {
            throw new FileOnLocalMachineNotFoundException(Path.GetDirectoryName(fullPath)!);
        }
    }

    public static void CheckIfFileNotExistsOnDisk(this string fullPath)
    {
        if (File.Exists(fullPath))
        {
            throw new FileAlreadyUploadedException(Path.GetDirectoryName(fullPath)!);
        }
    }

    public static string GetFullPath(this FileMetadata fileMetadata)
    {
        return fileMetadata?.Path != null
            ? Path.Combine(fileMetadata.Path, fileMetadata.ActualName)
            : string.Empty;
    }

    public static void CheckIfFileIsImage(this FileTypes fileType)
    {
        if (fileType != FileTypes.Image)
        {
            throw new InvalidFileTypeException(fileType!.ToString());
        }
    }

    public static void CheckIfFileIsVideo(this FileTypes fileType)
    {
        if (fileType != FileTypes.Video)
        {
            throw new InvalidFileTypeException(fileType!.ToString());
        }
    }
}