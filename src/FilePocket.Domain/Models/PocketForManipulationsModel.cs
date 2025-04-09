namespace FilePocket.Domain.Models;

public class PocketForManipulationsModel
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDefault { get; set; } = false;
}
