namespace FilePocket.Domain.Models.Configuration;

public class JwtConfigurationModel
{
    public string ValidIssuer { get; set; } = string.Empty;

    public string ValidAudience { get; set; } = string.Empty;

    public string TokenKey { get; set; } = string.Empty;

    public string Expires { get; set; } = string.Empty;
}
