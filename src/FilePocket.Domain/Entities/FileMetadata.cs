using FilePocket.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Entities;

[Index(nameof(ActualName), IsUnique = true)]
public class FileMetadata
{
    public Guid Id { get; set; }

    [Required]
    public string? OriginalName { get; set; }

    public string ActualName { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string? Path { get; set; }

    [Required]
    public FileTypes? FileType { get; set; }

    [Required]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    [Required]
    public double FileSize { get; set; }

    public Guid? PocketId { get; set; }

    [Required]
    public Guid UserId { get; set; }
    
    public Guid? FolderId { get; set; }

    public bool IsDeleted { get; set; }
}
