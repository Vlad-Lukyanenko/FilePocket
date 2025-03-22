namespace FilePocket.BlazorClient.Features.Profiles.Models;

public class ProfileModel
{
    public Guid Id { get; init; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public Guid? IconId { get; set; }
}
