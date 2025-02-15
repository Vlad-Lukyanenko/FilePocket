using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Models;

public class PocketForManipulationsModel
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsDefault { get; set; } = false;
}
