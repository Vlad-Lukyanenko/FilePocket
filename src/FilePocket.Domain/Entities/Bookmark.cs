namespace FilePocket.Domain.Entities;

public class Bookmark
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid PocketId { get; set; }
    public Guid? FolderId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public virtual Pocket Pocket { get; init; }
    public virtual Folder Folder { get; init; }
}
