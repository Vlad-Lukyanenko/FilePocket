using System.ComponentModel.DataAnnotations;

namespace FilePocket.BlazorClient.Services.Authentication.Models;

public class RegistrationRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    [RegularExpression(@"^\S*\d\S*$", ErrorMessage = "Passwords must have at least one digit ('0'-'9').")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string? ConfirmPassword { get; set; }
}
