namespace FilePocket.BlazorClient.Features.Profiles.Models;

public class UpdateProfileModel
{
    public Guid Id { get; init; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public Guid? IconId { get; set; }

    public string? PhoneNumber { get; set; } 
    public string? Language { get; set; } 
    public DateTime? BirthDate { get; set; } 
}
