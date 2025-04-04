using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace FilePocket.WebApi.Controllers;

[Route("api/trash/")]
[ApiController]
[ServiceFilter(typeof(JwtOrApiKeyAuthorizeAttribute))]
public class TrashController : BaseController
{
    private readonly IServiceManager _service;

    public TrashController(IServiceManager service)
    {
        _service = service;
    }

    [HttpPut("files/{fileId:guid}")]
    public async Task<IActionResult> MoveFileToTrash([FromRoute] Guid fileId)
    {
        await _service.FileService.MoveToTrash(UserId, fileId);

        return Ok();
    }

    [HttpPut("pockets/{pocketId:guid}")]
    public async Task<IActionResult> MovePocketToTrash([FromRoute] Guid pocketId)
    {
        await _service.PocketService.MoveToTrash(UserId, pocketId);

        return Ok();
    }
}
