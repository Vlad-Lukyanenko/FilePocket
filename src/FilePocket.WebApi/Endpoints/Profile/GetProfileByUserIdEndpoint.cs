using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Profile;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.Profile;

public class GetProfileByUserIdEndpoint : BaseEndpointWithoutRequest<GetProfileResponse>
{
    private readonly IServiceManager _service;
    private readonly IMapper _mapper;

    public GetProfileByUserIdEndpoint(IServiceManager service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Get("api/profile/userId/{userId:guid}");

        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var userId = Route<Guid>("userId");

        var profile = await _service.ProfileService.GetByUserIdAsync(userId);
        var response = _mapper.Map<GetProfileResponse>(profile);

        await SendOkAsync(response, cancellationToken);
    }
}
