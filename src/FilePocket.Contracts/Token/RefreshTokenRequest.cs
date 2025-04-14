namespace FilePocket.Contracts.Token;

public class RefreshTokenRequest
{
    public bool IsSuccess { get; set; }
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
