namespace FilePocket.BlazorClient.Features.Files.Models;

public class UpdateFileInfoModel
{
    public Guid Id { get; set; }
    public string? OriginalName { get; set; }
    public Guid? PocketId { get; set; }
    public Guid? FolderId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
