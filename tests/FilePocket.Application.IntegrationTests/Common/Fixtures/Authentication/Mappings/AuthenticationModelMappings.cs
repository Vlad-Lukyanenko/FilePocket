using FilePocket.Domain.Models;

namespace FilePocket.Application.IntegrationTests.Common.Fixtures.Authentication.Mappings;

internal static class AuthenticationModelMappings
{
    internal static UserLoginModel ToUserLoginModel(this UserRegistrationModel userRegistrationModel)
        => new() { Email = userRegistrationModel.Email, Password = userRegistrationModel.Password };
}