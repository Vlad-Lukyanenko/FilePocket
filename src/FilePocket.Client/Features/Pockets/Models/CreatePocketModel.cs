using System.ComponentModel.DataAnnotations;

namespace FilePocket.Client.Services.Pockets.Models;

public class CreatePocketModel
{
    public string Name { get; set; } = string.Empty;

    public Guid UserId { get; set; }
    [StringLength(500, ErrorMessage = "Description must be less than 500 characters.")]
<<<<<<< Updated upstream
    public string Description { get; set; } = string.Empty;
=======
    public string? Description { get; set; } 
>>>>>>> Stashed changes
}
