namespace FilePocket.Contracts.Bookmark;

public class CreateBookmarkResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
