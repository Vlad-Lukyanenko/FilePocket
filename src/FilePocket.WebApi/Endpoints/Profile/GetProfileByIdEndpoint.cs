using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Profile;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.Profile;

public class GetProfileByIdEndpoint : BaseEndpointWithoutRequest<GetProfileResponse>
{
    private readonly IServiceManager _service;
    private readonly IMapper _mapper;

    public GetProfileByIdEndpoint(IServiceManager service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Get("api/profile/{id:guid}");

        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var id = Route<Guid>("id");

        var profile = await _service.ProfileService.GetByIdAsync(id);
        var response = _mapper.Map<GetProfileResponse>(profile);

        await SendOkAsync(response, cancellationToken);
    }
}
