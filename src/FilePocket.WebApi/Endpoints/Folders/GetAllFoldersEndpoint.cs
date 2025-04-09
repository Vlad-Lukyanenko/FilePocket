using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Folders.Responses;
using FilePocket.Domain.Enums;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;
using static System.Net.WebRequestMethods;

namespace FilePocket.WebApi.Endpoints.Folders;

public class GetAllFoldersEndpoint : BaseEndpointWithoutRequest<List<GetAllFoldersResponse>>
{
    private readonly IServiceManager _service;
    private readonly IMapper _mapper;

    public GetAllFoldersEndpoint(IServiceManager service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Verbs(Http.Get);
        Routes("api/pockets/{pocketId:guid}/{folderType}/{isSoftDeleted:bool}/folders");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var folderType = Route<FolderType>("folderType");
        var isSoftDeleted = Route<bool>("isSoftDeleted");
        var folders = await _service.FolderService.GetAllAsync(UserId, PocketId, null, folderType, isSoftDeleted);

        var response = new List<GetAllFoldersResponse>();
        foreach (var folder in folders)
        {
            response.Add(_mapper.Map<GetAllFoldersResponse>(folder));
        }

        await SendOkAsync(response, cancellationToken);
    }
}
