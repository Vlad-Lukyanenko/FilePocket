using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Models;

public class PocketModel
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsDefault { get; set; } = false;

    [Required]
    public Guid UserId { get; set; }

    public int NumberOfFiles { get; set; } = 0;

    public double TotalSize { get; set; } = 0;
}
