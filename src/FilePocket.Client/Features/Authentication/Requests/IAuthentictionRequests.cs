using FilePocket.Client.Services.Authentication.Models;

namespace FilePocket.Client.Services.Authentication.Requests
{
    public interface IAuthentictionRequests
    {
        Task<bool> RegisterUserAsync(RegistrationRequest registrationModel);

        Task<TokenModel> LoginAsync(LoginModel loginModel);
        
        Task Logout();
    }
}
