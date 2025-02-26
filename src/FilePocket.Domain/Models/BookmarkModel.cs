using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Models;

public class BookmarkModel
{
    public Guid Id { get; set; }

    [Required]
    public Guid PocketId { get; set; }
    public Guid? FolderId { get; set; }
    public Guid UserId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Url { get; set; } = string.Empty;
}
