using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace FilePocket.Contracts.Services;

public interface IAuthenticationService
{
    Task<RegisterUserResponse> RegisterUser(UserRegistrationModel userForRegistration);

    Task<bool> ValidateUser(UserLoginModel userLoginModel);

    Task<TokenModel> CreateToken(bool populateExp);

    Task<TokenModel> RefreshToken(TokenModel tokenModel);
}
