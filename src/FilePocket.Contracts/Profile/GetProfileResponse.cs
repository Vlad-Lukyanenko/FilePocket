namespace FilePocket.Contracts.Profile;

public class GetProfileResponse
{
    public Guid Id { get; init; }

    public string? FirstName { get; init; }

    public string? LastName { get; init; }

    public string? Email { get; init; }

    public Guid? IconId { get; init; }

    public DateTime CreatedAt { get; init; }

    public Guid UserId { get; init; }
}
