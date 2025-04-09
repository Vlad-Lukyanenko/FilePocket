using FilePocket.Domain.Entities.Abstractions;
using System.Text.Json.Serialization;

namespace FilePocket.Domain.Entities;

public class Bookmark : IAmSoftDeletedEntity
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid PocketId { get; set; }
    public Guid? FolderId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty; 
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    [JsonIgnore]
    public virtual Pocket Pocket { get; init; }
    [JsonIgnore]
    public virtual Folder Folder { get; init; }

    public void MarkAsDeleted(DateTime? deletedAt = null)
    {
        IsDeleted = true;
        DeletedAt = deletedAt ?? DateTime.UtcNow;
    }
}
