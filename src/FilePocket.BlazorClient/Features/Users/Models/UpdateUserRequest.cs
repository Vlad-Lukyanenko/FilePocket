namespace FilePocket.BlazorClient.Features.Users.Models
{
    public class UpdateUserRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
    }
}
