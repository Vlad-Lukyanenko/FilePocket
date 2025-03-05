using System.ComponentModel.DataAnnotations;

namespace FilePocket.Contracts.Bookmark;

public class CreateBookmarkRequest
{
    [Required]
    public Guid PocketId { get; set; }
    public Guid? FolderId { get; set; }
    public Guid UserId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
