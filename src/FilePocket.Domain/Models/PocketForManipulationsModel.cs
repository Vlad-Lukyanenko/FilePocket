using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Models;

public class PocketForManipulationsModel
{
    [Required]
    public string? Name { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public Guid UserId { get; set; }
}
