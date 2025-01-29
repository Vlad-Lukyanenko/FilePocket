using FilePocket.Client.Features.Users.Models;

namespace FilePocket.Client.Features.Users.Requests
{
    public interface IUserRequests
    {
        Task<LoggedInUserModel?> GetByUserNameAsync(string userName);

        Task UpdateUserAsync(UpdateUserRequest updateUserRequest);
    }
}
