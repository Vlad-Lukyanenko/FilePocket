using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Models;

public class StorageForManipulationsModel
{
    [Required]
    public string? Name { get; set; }

    [Required]
    public Guid UserId { get; set; }
}
