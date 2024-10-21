namespace FilePocket.Domain.Models;

public class JwtConfigurationModel
{
    public string? ValidIssuer { get; set; }

    public string? ValidAudience { get; set; }

    public string? TokenKey { get; set; }

    public string? Expires { get; set; }
}
