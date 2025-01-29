namespace FilePocket.Domain.Models;

public class FileResponseModel
{
    public Guid Id { get; set; }

    public byte[]? FileByteArray { get; set; }

    public string? OriginalName { get; set; }

    public Guid ActualName { get; set; }

    public FileTypes? FileType { get; set; }

    public DateTime DateCreated { get; set; }

    public Guid? PocketId { get; set; }

    public double? FileSize { get; set; }

    public string? PocketName { get; set; }
    public Guid? FolderId { get; set; }
    public Guid? UserId { get; set; }
}
