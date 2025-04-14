namespace FilePocket.Contracts.User;

public class UpdateUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string? Language { get; set; } = string.Empty;
}
