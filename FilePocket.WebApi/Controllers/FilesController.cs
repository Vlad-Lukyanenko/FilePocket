using FilePocket.Contracts.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using FilePocket.Shared.Claims;
using FilePocket.Shared.Exceptions;
using MagpieChat.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FilePocket.WebApi.Controllers;

[Route("api/files/")]
[ApiController]
//[Authorize]
public class FilesController : ControllerBase
{
    private readonly IServiceManager _service;

    public FilesController(IServiceManager service)
    {
        _service = service;
    }

    #region GET
    [HttpGet("ping")]
    [AllowAnonymous]
    public IActionResult Ping()
    {
        return Ok("Pong");
    }

    [HttpGet("pocket/{pocketId}", Name = "All")]
    public async Task<IActionResult> GetAll([FromRoute] Guid pocketId)
    {
        var fileUploadSummaries = await _service.FileService.GetAllFilesByStorageIdAsync(pocketId);

        return Ok(fileUploadSummaries);
    }

    [HttpGet("filtered")]
    public async Task<IActionResult> GetFilteredFiles([FromQuery] FilesFilterOptionsModel? filterOptionsModel)
    {
        if (filterOptionsModel == null)
        {
            return BadRequest();
        }

        var fileUploadSummaries = new List<FileResponseModel>();

        fileUploadSummaries.AddRange(await _service.FileService.GetFilteredFilesAsync(filterOptionsModel));

        var itemsCount = await _service.FileService.GetFilteredFilesCountAsync(filterOptionsModel);
        var pagesCount = Math.Ceiling(itemsCount / (decimal)filterOptionsModel.PageSize);

        return Ok(new
        {
            Files = fileUploadSummaries,
            PagesCount = pagesCount,
            ItemsCount = itemsCount
        });

    }

    [HttpGet("check")]
    public async Task<IActionResult> CheckIfFileExists(string fileName, Guid storageId)
    {
        var response = await _service.FileService.CheckIfFileExists(fileName, storageId);

        return Ok(response);
    }

    [HttpGet("{fileId:guid}/storages/{storageId:guid}")]
    public async Task<IActionResult> Get(Guid storageId, Guid fileId)
    {
        var file = await _service.FileService.GetFileByIdAsync(storageId, fileId);

        return Ok(file);
    }

    [HttpGet("storage/{storageId:guid}/image/thumbnail")]
    //[AllowAnonymous]
    public async Task<IActionResult> GetImageThumbnail(
        [FromRoute, Required] Guid storageId,
        [FromQuery, Required] Guid id,
        [FromQuery, Required] int size)
    {
        var image = await _service.FileService.GetImageThumbnailAsync(storageId, id, size);

        return Ok(image);
    }
    #endregion

    #region POST
    [HttpPost]
    public async Task<IActionResult> UploadFile([FromForm] FileInformation fileInformation)
    {
        var validationResult = ValidateFile(fileInformation);
        if (validationResult is not null)
        {
            return validationResult;
        }

        try
        {
            var fileUploadSummary = await _service.FileService.UploadFileAsync(fileInformation.File!, fileInformation.ClientId!.Value, fileInformation.StorageId!.Value);

            return CreatedAtRoute(
                "FileByUploadSummaryId",
                new
                {
                    fileInformation.StorageId,
                    id = fileUploadSummary.Id
                }, fileUploadSummary);
        }
        catch (FileAlreadyUploadedException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{storageId:guid}")]
    public async Task<IActionResult> Create([FromForm] IFormFile? file, [FromRoute] Guid? storageId)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("Nothing to upload.");
        }

        if (storageId is null)
        {
            return BadRequest("StorageId cannot be null");
        }

        try
        {
            var userId = GetUserId();

            var fileUploadSummary = await _service.FileService.UploadFileAsync(file, userId, storageId.Value);

            return CreatedAtRoute("FileByUploadSummaryId", new { storageId, id = fileUploadSummary.Id }, fileUploadSummary);

        }
        catch (FileAlreadyUploadedException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("session/user/{userId:guid}/storage/{storageId:guid}")]
    public IActionResult StartSession([FromRoute] Guid userId, [FromRoute] Guid storageId, [FromBody] CreateSessionParams sessionParams)
    {
        var session = _service.FileService.CreateSession(
                userId,
                storageId,
                sessionParams);

        return Ok(new
        {
            FileName = sessionParams.FileName!,
            session!.Id,
            session!.UserId,
            session!.StorageId,
            session!.FileInfo.ChunkSize,
        });
    }

    [HttpPost("upload/user/{userId}/storage/{storageId}/session/{sessionId}")]
    public async Task<IActionResult> UploadFileChunk(
        [FromForm] IFormFile file,
        [FromRoute, Required] string userId,
        [FromRoute, Required] string storageId,
        [FromRoute, Required] string sessionId,
        [FromQuery, Required] int chunkNumber)

    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest("User missing");

        if (string.IsNullOrWhiteSpace(storageId))
            return BadRequest("Stroage ID is missing");

        if (string.IsNullOrWhiteSpace(sessionId))
            return BadRequest("Session ID is missing");

        if (chunkNumber < 1)
            return BadRequest("Invalid chunk number");

        await _service.FileService.UploadFileChunkAsync(file!, userId, storageId, sessionId, chunkNumber);

        return Ok();
    }

    [HttpPost("storage/{storageId:guid}/image/thumbnails")]
    public async Task<IActionResult> GetImageThumbnails(
    [FromBody, Required] List<UserIconInfoRequest> request,
    [FromRoute, Required] Guid storageId,
    [FromQuery, Required] int size)
    {
        var images = await _service.FileService.GetImageThumbnailsAsync(request, storageId, size);

        return CreatedAtRoute("All", new { storageId }, images!);
    }
    #endregion

    #region DELETE
    [HttpDelete("{id:guid}")]
    //[Authorize(Roles = "Administrator")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid storageId, Guid id)
    {
        await _service.FileService.DeleteFileAsync(storageId, id);

        return NoContent();
    }
    #endregion


    private Guid GetUserId()
    {
        var uid = User.FindFirst(CustomClaimTypes.UserId)!.Value;

        return Guid.Parse(uid);
    }

    private BadRequestObjectResult? ValidateFile(FileInformation fileInformation)
    {
        if (fileInformation.File is null || fileInformation.File.Length == 0)
        {
            return BadRequest("Nothing to upload.");
        }

        if (fileInformation.StorageId is null)
        {
            return BadRequest("StorageId cannot be null");
        }

        return null;
    }

    private static byte[] ToByteArray(Stream stream)
    {
        using (var ms = new MemoryStream())
        {
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }

    public class FileInformation
    {
        public Guid? ClientId { get; set; }
        public Guid? StorageId { get; set; }
        public IFormFile? File { get; set; }
    }
}
