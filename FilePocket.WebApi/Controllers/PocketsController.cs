using FilePocket.Contracts.Services;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilePocket.WebApi.Controllers;

[Route("api/pockets")]
[ApiController]
//[Authorize]
public class PocketsController : ControllerBase
{
    private readonly IServiceManager _service;

    public PocketsController(IServiceManager service)
    {
        _service = service;
    }

    [HttpGet("ping")]
    [AllowAnonymous]
    public IActionResult Ping()
    {
        return Ok("Pong");
    }

    [HttpGet("all/users/{userId:Guid}")]
    public async Task<IActionResult> GetAll([FromRoute] Guid userId)
    {
        var storages = await _service.StorageService.GetAllByUserIdAsync(userId, trackChanges: false);

        return Ok(storages);
    }

    [HttpGet("{pocketId:guid}/info")]
    public async Task<IActionResult> GetDetails([FromRoute] Guid pocketId)
    {
        var storage = await _service.StorageService.GetStorageDetailsAsync(pocketId, trackChanges: false);

        return Ok(storage);
    }

    [HttpGet("{pocketId:guid}/checksize")]
    public async Task<IActionResult> CheckFileSize([FromRoute] Guid pocketId, [FromQuery] double fileSize)
    {
        var canUpload = await _service.StorageService.GetComparingDefaultCapacityWithTotalFilesSizeInStorage(pocketId, fileSize);

        return Ok(canUpload);
    }

    [HttpGet("{pocketId:guid}")]
    public async Task<IActionResult> Get(Guid pocketId)
    {
        var storage = await _service.StorageService.GetByIdAsync(pocketId, trackChanges: false);

        return Ok(storage);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StorageForManipulationsModel pocket)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        var createdStorage = await _service.StorageService.CreateStorageAsync(pocket);

        return Ok(createdStorage);
    }

    [HttpPut("{pocketId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid pocketId, [FromBody] StorageForManipulationsModel storage)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        await _service.StorageService.UpdateStorageAsync(pocketId, storage, trackChanges: true);

        return NoContent();
    }

    [HttpDelete("{pocketId:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid pocketId)
    {
        await _service.FolderService.DeleteByPocketIdAsync(pocketId);
        await _service.StorageService.DeleteStorageAsync(pocketId, trackChanges: false);

        return NoContent();
    }
}
