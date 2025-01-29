using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Models;

public class PocketModel
{
    public Guid Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public int? NumberOfFiles { get; set; }

    public double? TotalSize { get; set; }
}
