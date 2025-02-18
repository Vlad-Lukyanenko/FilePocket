namespace FilePocket.BlazorClient.Services.Authentication.Models;

public class TokenModel
{
    public bool IsSuccess { get; set; }
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
