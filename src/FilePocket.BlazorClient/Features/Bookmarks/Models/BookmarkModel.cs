using System.ComponentModel.DataAnnotations;

namespace FilePocket.BlazorClient.Features.Bookmarks.Models;

public class BookmarkModel
{
    public Guid Id { get; set; }
    public Guid PocketId { get; set; }
    public Guid? FolderId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string OriginalTitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
