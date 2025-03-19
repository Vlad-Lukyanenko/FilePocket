namespace FilePocket.Domain.Entities;

public class Profile
{
    public Guid Id { get; init; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; } = string.Empty;

    public Guid? IconId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid UserId { get; set; }
}
