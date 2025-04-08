using FilePocket.Contracts.User;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
namespace FilePocket.WebApi.Endpoints.User;

public class GetUserByNameEndpoint : BaseEndpointWithoutRequest<GetUserResponse>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IMapper _mapper;

    public GetUserByNameEndpoint(UserManager<Domain.Entities.User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Get("api/users/{name:string}");
        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var name = Route<string>("name");

        if (name is not null)
        {
            var user = await _userManager.FindByNameAsync(name);
            var response = _mapper.Map<GetUserResponse>(user!);

            await SendOkAsync(response, cancellationToken);
            return;
        }

        await SendErrorsAsync();
    }
}
