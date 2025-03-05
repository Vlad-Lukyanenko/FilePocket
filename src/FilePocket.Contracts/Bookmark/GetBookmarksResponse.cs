using System.ComponentModel.DataAnnotations;

namespace FilePocket.Contracts.Bookmark;

public class GetBookmarksResponse
{
    public Guid Id { get; set; }
    public Guid PocketId { get; set; }
    public Guid? FolderId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
