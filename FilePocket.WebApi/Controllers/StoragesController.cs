using FilePocket.Application.Services;
using FilePocket.Contracts.Services;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilePocket.WebApi.Controllers;

[Route("api/storages")]
[ApiController]
//[Authorize]
public class StoragesController : ControllerBase
{
    private readonly IServiceManager _service;

    public StoragesController(IServiceManager service)
    {
        _service = service;
    }

    [HttpGet("ping")]
    [AllowAnonymous]
    public IActionResult Ping()
    {
        return Ok("Pong");
    }

    [HttpGet("all/user/{userId:Guid}")]
    public async Task<IActionResult> GetAll([FromRoute] Guid userId)
    {
        var storages = await _service.StorageService.GetAllByUserIdAsync(userId, trackChanges: false);

        return Ok(storages);
    }

    [HttpGet("info/{id:guid}")]
    public async Task<IActionResult> GetDetails(Guid id)
    {
        var storage = await _service.StorageService.GetStorageDetailsAsync(id, trackChanges: false);

        return Ok(storage);
    }

    [HttpGet("checksize/{id:guid}")]
    public async Task<IActionResult> CheckFileSize(Guid id, [FromQuery] double fileSize)
    {
        var canUpload = await _service.StorageService.GetComparingDefaultCapacityWithTotalFilesSizeInStorage(id, fileSize);
        return Ok(canUpload);
    }

    [HttpGet("{id:guid}", Name = "StorageById")]
    public async Task<IActionResult> Get(Guid id)
    {
        var storage = await _service.StorageService.GetByIdAsync(id, trackChanges: false);

        return Ok(storage);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StorageForManipulationsModel storage)
    {
        if (storage is null)
        {
            return BadRequest("Storage object to create is null");
        }

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        var createdStorage = await _service.StorageService.CreateStorageAsync(storage);

        return CreatedAtRoute("StorageById", new { id = createdStorage.Id }, createdStorage);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] StorageForManipulationsModel storage)
    {
        if (storage is null)
        {
            return BadRequest("Storage object to update is null");
        }

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        await _service.StorageService.UpdateStorageAsync(id, storage, trackChanges: true);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.StorageService.DeleteStorageAsync(id, trackChanges: false);

        return NoContent();
    }
}
