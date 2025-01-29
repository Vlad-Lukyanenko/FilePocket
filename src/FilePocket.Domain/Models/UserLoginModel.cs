using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Models;

public class UserLoginModel
{
    [Required(ErrorMessage = "User name is required")]
    public string? Email { get; init; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; init; }
}
