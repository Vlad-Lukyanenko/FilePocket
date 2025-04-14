using FilePocket.Contracts.User;
using FilePocket.WebApi.Endpoints.Base;
using Microsoft.AspNetCore.Identity;

namespace FilePocket.WebApi.Endpoints.User;

public class UpdateUserEndpoint : BaseEndpointWithoutResponse<UpdateUserRequest>
{
    private readonly UserManager<Domain.Entities.User> _userManager;

    public UpdateUserEndpoint(UserManager<Domain.Entities.User> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Put("api/users");
        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(UpdateUserRequest user, CancellationToken cancellationToken)
    {
        var userToUpdate = await _userManager.FindByNameAsync(user.UserName);

        if (userToUpdate is not null)
        {
            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;

            await _userManager.UpdateAsync(userToUpdate);
        }

        await SendNoContentAsync();
    }
}
