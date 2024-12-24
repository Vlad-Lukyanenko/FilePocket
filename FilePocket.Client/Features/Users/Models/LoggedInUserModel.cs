namespace FilePocket.Client.Features.Users.Models
{
    public class LoggedInUserModel
    {
        public Guid? Id { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public ICollection<string>? Roles { get; set; }
    }
}
