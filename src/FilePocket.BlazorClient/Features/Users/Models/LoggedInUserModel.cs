using FilePocket.BlazorClient.Features.Profiles.Models;

namespace FilePocket.BlazorClient.Features.Users.Models
{
    public class LoggedInUserModel
    {
        public Guid? Id { get; set; }

        public string? UserName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public ICollection<string>? Roles { get; set; }

        public ProfileModel? Profile { get; set; }
    }
}
