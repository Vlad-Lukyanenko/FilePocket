using System.ComponentModel.DataAnnotations;

namespace FilePocket.Client.Services.Authentication.Models;

public class RegistrationRequest
{
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }

    [Required]
    public string? ConfirmPassword { get; set; }
}
