using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Entities;

public class Pocket
{
    public Guid Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public int? NumberOfFiles { get; set; }

    public double? TotalSize { get; set; }

    [Required]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    [MaxLength(500)]
    public string? Description { get; set; }
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
    public virtual ICollection<FileMetadata>? FileMetadata { get; set; }
}
