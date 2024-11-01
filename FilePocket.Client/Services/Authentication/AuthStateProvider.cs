using Microsoft.AspNetCore.Components.Authorization;

namespace FilePocket.Client.AuthFeatures;

public class AuthStateProvider : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        throw new NotImplementedException();
    }
}
