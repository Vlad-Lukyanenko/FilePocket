using System.ComponentModel.DataAnnotations;

namespace FilePocket.Contracts.Pocket;

public class CreateAndUpdatePocketRequest
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDefault { get; set; } = false;
}
