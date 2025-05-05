using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Token;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.Token;

public class RefreshTokenEndpoint : BaseEndpoint<RefreshTokenRequest, RefreshTokenResponse>
{
    private readonly IServiceManager _service;
    private readonly IMapper _mapper;

    public RefreshTokenEndpoint(IServiceManager service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Post("api/token/refresh");
    }

    public override async Task HandleAsync(RefreshTokenRequest token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            await SendErrorsAsync();
        }
        else
        {
            var tokenToRefresh = _mapper.Map<TokenModel>(token);
            var tokenDtoToReturn = await _service.AuthenticationService.RefreshToken(tokenToRefresh);
            if(tokenDtoToReturn == null)
            {
                await SendUnauthorizedAsync(cancellationToken);
            }
            else
            {
                var response = _mapper.Map<RefreshTokenResponse>(tokenDtoToReturn);
                await SendOkAsync(response, cancellationToken);
            }
        }            
    }
}
