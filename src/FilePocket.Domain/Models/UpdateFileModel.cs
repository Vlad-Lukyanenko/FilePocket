namespace FilePocket.Domain.Models;

public class UpdateFileModel
{
    public Guid Id { get; init; }
    public string OriginalName { get; init; }
    public Guid UserId { get; set; }
    public Guid PocketId { get; init; }
    public Guid? FolderId { get; init; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
