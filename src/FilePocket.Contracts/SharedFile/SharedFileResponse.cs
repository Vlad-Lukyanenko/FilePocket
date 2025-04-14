using FilePocket.Domain;

namespace FilePocket.Contracts.SharedFile;

public class SharedFileResponse
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FileName { get; set; }
    public double FileSize { get; set; }
    public FileTypes? FileType { get; set; }
    public Guid FileId { get; set; }
    public Guid? PocketId { get; set; }
    public DateTime CreatedAt { get; set; }
}
