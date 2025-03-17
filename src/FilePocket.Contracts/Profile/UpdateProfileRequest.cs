namespace FilePocket.Contracts.Profile;

public class UpdateProfileRequest
{
    public Guid Id { get; init; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public Guid? IconId { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
