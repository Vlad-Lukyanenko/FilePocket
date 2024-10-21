using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Models;

public class StorageModel
{
    public Guid Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public Guid UserId { get; set; }
}
