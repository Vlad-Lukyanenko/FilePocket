using Microsoft.Extensions.Configuration;

namespace FilePocket.Domain.Models.Configuration
{
    public class ClientAppsRequestHeaderSettings
    {
        public string HeaderName { get; set; } = string.Empty;
        public string HeaderValue { get; set; } = string.Empty;
    }
}
