using FilePocket.BlazorClient.Features.Users.Models;

namespace FilePocket.BlazorClient.Features.Users.Requests
{
    public interface IUserRequests
    {
        Task<LoggedInUserModel?> GetByUserNameAsync(string userName);

        Task UpdateUserAsync(UpdateUserRequest updateUserRequest);
    }
}
