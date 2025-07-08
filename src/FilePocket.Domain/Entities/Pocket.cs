using FilePocket.Domain.Entities.Abstractions;
using FilePocket.Domain.Extensions;

namespace FilePocket.Domain.Entities;

public class Pocket : IAmSoftDeletedEntity
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int NumberOfFiles { get; set; }
    public double TotalSize { get; set; }
    public bool IsDefault { get; init; }
    public bool IsDeleted { get; private set; }
    public DateTime DateCreated { get; init; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; private set; }
    public virtual ICollection<FileMetadata>? FileMetadata { get; init; }
    public virtual ICollection<Folder>? Folders { get; init; }

    public void UpdateDetails(FileMetadata fileMetadata)
    {
        NumberOfFiles++;
        TotalSize += fileMetadata.FileSize;
    }

    public void UpdateDetails(double sizeChange)
    {
        NumberOfFiles++;
        TotalSize += sizeChange;
    }

    public void MarkAsDeleted(DateTime? deletedAt = null)
    {
        IsDeleted = true;
        DeletedAt = deletedAt ?? DateTime.UtcNow;

        if (FileMetadata is not null && FileMetadata.Any())
        {
            FileMetadata?.ForEach(b => b.MarkAsDeleted(DeletedAt));
        }

        if (Folders is not null && Folders.Any())
        {
            Folders?.ForEach(b => b.MarkAsDeleted(DeletedAt));
        }
    }

    public void RestoreFromDeleted()
    {
        IsDeleted = false;
        DeletedAt = null;
        if (FileMetadata is not null && FileMetadata.Any())
        {
            FileMetadata?.ForEach(f =>
            {
                f.RestoreFromDeleted();
            });
        }
        if (Folders is not null && Folders.Any())
        {
            Folders?.ForEach(f =>
            {
                f.RestoreFromDeleted();
            });
        }
    }
}
