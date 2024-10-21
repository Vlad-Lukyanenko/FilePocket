using FilePocket.Admin.Models.Authentication;

namespace FilePocket.Admin.Requests.Contracts;

public interface IAuthentictionRequests
{
    Task<bool> RegisterUserAsync(RegistrationModel registrationModel);

    Task<TokenModel> LoginUserAsync(LoginModel loginModel);

    Task<TokenModel> RefreshAccessTokenAsync(TokenModel tokenModel);
}
