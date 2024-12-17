using FilePocket.Client.Features.Users.Models;

namespace FilePocket.Admin.Requests.Contracts
{
    public interface IUserRequests
    {
        Task<LoggedInUserModel?> GetByUserNameAsync(string userName);
    }
}
