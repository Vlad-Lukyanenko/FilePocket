namespace FilePocket.Domain.Models;

public class BookmarkModel
{
    public Guid Id { get; set; }
    public Guid PocketId { get; set; }
    public Guid? FolderId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
