namespace FilePocket.Domain.Models;

public class TokenModel
{
    //public string Issuer { get; set; } = string.Empty;

    //public string Audience { get; set; } = string.Empty;

    //public string AccessToken { get; set; } = string.Empty;

    //public string RefreshToken { get; set; } = string.Empty;

    public bool IsSuccess { get; set; }
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
