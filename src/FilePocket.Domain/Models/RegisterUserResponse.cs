using FilePocket.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FilePocket.Domain.Models
{
    public class RegisterUserResponse
    {
        public IdentityResult? IdentityResult { get; set; }

        public User? User { get; set; }
    }
}
