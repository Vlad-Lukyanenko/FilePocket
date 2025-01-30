using FilePocket.Client.Services.Authentication.Models;

namespace FilePocket.Client.Services.Authentication.Requests
{
    public interface IAuthentictionRequests
    {
        Task<HttpResponseMessage> RegisterUserAsync(RegistrationRequest registrationModel);

        Task<TokenModel> LoginAsync(LoginModel loginModel);
        
        Task Logout();
    }
}
