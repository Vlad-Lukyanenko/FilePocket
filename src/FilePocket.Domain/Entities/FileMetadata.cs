using FilePocket.Domain.Entities.Abstractions;
using FilePocket.Domain.Models;

namespace FilePocket.Domain.Entities;

public class FileMetadata : IAmSoftDeletedEntity
{
    public FileMetadata() { }

    private FileMetadata(
        Guid id, Guid userId,
        string originalName, string actualName,
        string path, FileTypes fileType, double fileSize,
        Guid pocketId, Guid? folderId,
        bool isDeleted, 
        DateTime createdAt, DateTime? deletedAt = null)
    {
        Id = id;
        UserId = userId;

        PocketId = pocketId;
        FolderId = folderId;

        OriginalName = originalName;
        ActualName = actualName;

        Path = path;
        FileType = fileType;
        FileSize = fileSize;

        IsDeleted = isDeleted;
        
        CreatedAt = createdAt;
        DeletedAt = deletedAt;
    }

    public Guid Id { get; init; }
    public string OriginalName { get; init; }
    public string ActualName { get; init; }
    public string Path { get; init; }
    public FileTypes FileType { get; init; }
    public double FileSize { get; init; }
    public Guid UserId { get; init; }
    public Guid PocketId { get; init; }
    public Guid? FolderId { get; init; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; set; }

    public void MarkAsDeleted(DateTime? deletedAt = null)
    {
        IsDeleted = true;
        DeletedAt = deletedAt ?? DateTime.UtcNow;
    }

    public static FileMetadata Create(
        Guid userId, string originalFileName,
        string filePath, FileTypes fileType,
        double fileSizeInMbs, Guid pocketId, Guid? folderId)
    {
        var fileId = Guid.NewGuid();
        var actualName = Guid.NewGuid().ToString();

        return new FileMetadata(
            fileId, userId, originalFileName, actualName,
            filePath, fileType, fileSizeInMbs, pocketId, folderId,
            isDeleted: false, DateTime.UtcNow);
    }
}
