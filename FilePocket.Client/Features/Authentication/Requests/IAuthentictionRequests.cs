using FilePocket.Client.Services.Authentication.Models;

namespace FilePocket.Client.Services.Authentication.Requests
{
    public interface IAuthentictionRequests
    {
        Task<bool> RegisterUserAsync(RegistrationModel registrationModel);

        Task<TokenModel> LoginUserAsync(LoginModel loginModel);

        Task<TokenModel> RefreshAccessTokenAsync(TokenModel tokenModel);
    }
}
