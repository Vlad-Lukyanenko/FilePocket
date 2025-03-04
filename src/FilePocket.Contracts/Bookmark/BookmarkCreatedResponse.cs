namespace FilePocket.Contracts.Bookmark;

public class BookmarkCreatedResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
