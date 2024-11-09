using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilePocket.Domain.Entities;

[Index(nameof(ActualName), IsUnique = true)]
public class FileUploadSummary
{
    public Guid Id { get; set; }

    [Required]
    public string? OriginalName { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string ActualName { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string? Path { get; set; }

    [Required]
    public string? FileType { get; set; }

    [Required]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    [Required]
    public double FileSize { get; set; }

    [Required]
    public Guid StorageId { get; set; }
    
    public Guid? FolderId { get; set; }

    public virtual Storage? Storage { get; set; }
}
