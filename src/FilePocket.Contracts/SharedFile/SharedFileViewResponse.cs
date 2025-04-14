using FilePocket.Domain;

namespace FilePocket.Contracts.SharedFile;

public class SharedFileViewResponse
{
    public Guid SharedFileId { get; set; }
    public FileTypes? FileType { get; set; } = null;
    public string? OriginalName { get; set; } = string.Empty;
    public double FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
}
