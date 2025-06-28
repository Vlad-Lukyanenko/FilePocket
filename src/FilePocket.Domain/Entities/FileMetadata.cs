using FilePocket.Domain.Entities.Abstractions;
using System.Text.Json.Serialization;

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
        DateTime createdAt)
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
    }

    public Guid Id { get; init; }
    public string OriginalName { get; set; }
    public string ActualName { get; init; }
    public string Path { get; set; }
    public FileTypes FileType { get; init; }
    public double FileSize { get; set; }
    public Guid UserId { get; init; }
    public Guid PocketId { get; set; }
    public Guid? FolderId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore]
    public virtual Folder Folder { get; init; }

    public void MarkAsDeleted(DateTime? deletedAt = null)
    {
        IsDeleted = true;
        DeletedAt = deletedAt ?? DateTime.UtcNow;
    }

    public void RestoreFromDeleted()
    {
        IsDeleted = false;
        DeletedAt = null;
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
