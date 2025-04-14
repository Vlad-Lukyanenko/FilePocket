using FastEndpoints;
using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.EndpointProcessors;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Pocket;

public class CheckFileSizeEndpoint : BaseEndpointWithoutRequest<bool>
{
    private readonly IServiceManager _service;

    public CheckFileSizeEndpoint(IServiceManager service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Get("api/pockets/{pocketId:guid}/checksize");
        PreProcessor<AuthorizationProcessor<EmptyRequest>>();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var pocketId = Route<Guid>("pocketId");
        var fileSize = Query<double>("fileSize");

        var canUpload = await _service.PocketService.GetComparingDefaultCapacityWithTotalFilesSizeInPocket(UserId, pocketId, fileSize);

        await SendOkAsync(canUpload, cancellationToken);
    }
}
