using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Profile;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Profile;

public class UpdateProfileEndpoint : BaseEndpointWithoutResponse<UpdateProfileRequest>
{
    private readonly IServiceManager _service;

    public UpdateProfileEndpoint(IServiceManager service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Put("api/profile");
        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(UpdateProfileRequest profile, CancellationToken cancellationToken)
    {
        await _service.ProfileService.UpdateProfileAsync(profile);

        await SendNoContentAsync();
    }
}
