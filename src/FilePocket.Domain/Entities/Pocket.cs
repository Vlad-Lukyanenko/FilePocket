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

    public void UpdateDetails(FileMetadata fileMetadata)
    {
        NumberOfFiles++;
        TotalSize += fileMetadata.FileSize;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        FileMetadata?.ForEach(f => f.MarkAsDeleted(DeletedAt));
    }
}
