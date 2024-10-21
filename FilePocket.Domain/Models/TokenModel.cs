namespace FilePocket.Domain.Models;

public class TokenModel
{
    public string? AccessToken { get; init; }

    public string? RefreshToken { get; init; }
}
