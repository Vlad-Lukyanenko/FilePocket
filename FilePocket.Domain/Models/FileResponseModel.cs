namespace FilePocket.Domain.Models;

public class FileResponseModel
{
    public Guid Id { get; set; }

    public byte[]? FileByteArray { get; set; }

    public string? OriginalName { get; set; }

    public Guid ActualName { get; set; }

    public string? FileType { get; set; }

    public DateTime DateCreated { get; set; }

    public Guid StorageId { get; set; }

    public double? FileSize { get; set; }

    public string? StorageName { get; set;}
}
