using FilePocket.Contracts.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace FilePocket.WebApi.Controllers;

[Route("api/pockets")]
[ApiController]
[ServiceFilter(typeof(JwtOrApiKeyAuthorizeAttribute))]
public class PocketsController : BaseController
{
    private readonly IServiceManager _service;

    public PocketsController(IServiceManager service)
    {
        _service = service;
    }

    [HttpGet("default")]
    public async Task<IActionResult> GetDefault()
    {
        var pockets = await _service.PocketService.GetDefaultByUserIdAsync(UserId, trackChanges: false);

        return Ok(pockets);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllCustom()
    {
        var pockets = await _service.PocketService.GetAllCustomByUserIdAsync(UserId, trackChanges: false);

        return Ok(pockets);
    }

    [HttpGet("{pocketId:guid}/info")]
    public async Task<IActionResult> GetDetails([FromRoute] Guid pocketId)
    {
        var pockets = await _service.PocketService.GetPocketDetailsAsync(UserId, pocketId, trackChanges: false);

        return Ok(pockets);
    }

    [HttpGet("{pocketId:guid}/checksize")]
    public async Task<IActionResult> CheckFileSize([FromRoute] Guid pocketId, [FromQuery] double fileSize)
    {
        var canUpload = await _service.PocketService.GetComparingDefaultCapacityWithTotalFilesSizeInPocket(UserId, pocketId, fileSize);

        return Ok(canUpload);
    }

    [HttpGet("{pocketId:guid}")]
    public async Task<IActionResult> Get(Guid pocketId)
    {
        var pockets = await _service.PocketService.GetByIdAsync(UserId, pocketId, trackChanges: false);

        return Ok(pockets);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PocketForManipulationsModel pocket)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        var createdPocket = await _service.PocketService.CreatePocketAsync(pocket);

        return Ok(createdPocket);
    }

    [HttpPut("{pocketId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid pocketId, [FromBody] PocketForManipulationsModel pocket)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        await _service.PocketService.UpdatePocketAsync(pocketId, pocket, trackChanges: true);

        return NoContent();
    }

    [HttpDelete("{pocketId:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid pocketId)
    {
        await _service.FolderService.DeleteByPocketIdAsync(pocketId);
        await _service.PocketService.DeletePocketAsync(UserId, pocketId, trackChanges: false);

        return NoContent();
    }
}
