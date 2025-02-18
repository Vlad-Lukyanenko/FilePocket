using FilePocket.BlazorClient.Services.Authentication.Models;

namespace FilePocket.BlazorClient.Services.Authentication.Requests
{
    public interface IAuthentictionRequests
    {
        Task<HttpResponseMessage> RegisterUserAsync(RegistrationRequest registrationModel);

        Task<TokenModel> LoginAsync(LoginModel loginModel);
        
        Task Logout();
    }
}
