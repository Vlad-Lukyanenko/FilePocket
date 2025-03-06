namespace FilePocket.BlazorClient.Features.Bookmarks.Models;

public class CreateBookmarkModel
{
    public Guid PocketId { get; set; }
    public Guid? FolderId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
