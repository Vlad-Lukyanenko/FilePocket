using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Entities;

public class Pocket
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid UserId { get; set; }

    public int NumberOfFiles { get; set; } = 0;

    public double TotalSize { get; set; } = 0;

    public bool IsDefault { get; set; }

    [Required]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Description { get; set; }

    public virtual ICollection<FileMetadata>? FileMetadata { get; set; }

    public void UpdateDetails(FileMetadata fileMetadata)
    {
        NumberOfFiles++;
        TotalSize++;
    }
}
