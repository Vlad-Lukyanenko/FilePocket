using FilePocket.Domain.Entities.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FilePocket.Domain.Entities;

public class Note : IAmSoftDeletedEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PocketId { get; set; }
    public Guid? FolderId { get; init; }
    public string Title { get; set; } = string.Empty;
    public byte[] EncryptedContent { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public void MarkAsDeleted(DateTime? deletedAt = null)
    {
        IsDeleted = true;
        DeletedAt = deletedAt ?? DateTime.UtcNow;
    }
}
