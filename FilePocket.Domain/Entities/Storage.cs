using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Entities;

public class Storage
{
    public Guid Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    public virtual ICollection<FileUploadSummary>? FileUploadSummaries { get; set; }
}
