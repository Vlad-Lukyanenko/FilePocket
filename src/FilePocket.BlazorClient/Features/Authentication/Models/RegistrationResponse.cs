namespace FilePocket.BlazorClient.Features.Authentication.Models
{
    public class RegistrationResponse
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
