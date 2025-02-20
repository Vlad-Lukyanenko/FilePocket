namespace FilePocket.WebApi.Middlewares.Upload.Tus;

public class TusConfigurationModel
{
    public static string Section => "TusSettings";
    /// <summary>
    /// Enable or disable the tus middleware.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// The URL path for the tus endpoint.
    /// </summary>
    public string UrlPath { get; set; } = "/files";

    /// <summary>
    /// Maximum allowed upload size (in megabytes). For example, 500 MB.
    /// </summary>
    public int MaxAllowedUploadSizeMb { get; set; } = 500;
}