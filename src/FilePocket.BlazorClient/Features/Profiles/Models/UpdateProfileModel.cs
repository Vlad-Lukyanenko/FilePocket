namespace FilePocket.BlazorClient.Features.Profiles.Models;

public class UpdateProfileModel
{
    public Guid Id { get; init; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public Guid? IconId { get; set; }
}
