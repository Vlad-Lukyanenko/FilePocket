using Microsoft.Extensions.Configuration;

namespace FilePocket.Domain.Models.Configuration
{
    public class ApiKeyConfigurationModel
    {
        public string HeaderName { get; set; } = string.Empty;
        public string HeaderValue { get; set; } = string.Empty;
    }
}
