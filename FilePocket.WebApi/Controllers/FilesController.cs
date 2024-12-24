using FilePocket.Contracts.Services;
using FilePocket.Domain.Models;
using FilePocket.Shared.Claims;
using FilePocket.Shared.Exceptions;
using FilePocket.WebApi.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;

namespace FilePocket.WebApi.Controllers;

[Route("api/")]
[ApiController]
[ServiceFilter(typeof(JwtOrApiKeyAuthorizeAttribute))]
public class FilesController : ControllerBase
{
    private readonly IServiceManager _service;

    public FilesController(IServiceManager service)
    {
        _service = service;
    }

    #region GET
    [HttpGet("files/ping")]
    [AllowAnonymous]
    public IActionResult Ping()
    {
        return Ok("Pong");
    }

    [HttpGet("pockets/{pocketId}/files", Name = "All")]
    public async Task<IActionResult> GetAllFromPocket([FromRoute] Guid pocketId)
    {
        var fileUploadSummaries = await _service.FileService.GetAllFilesFromPocketAsync(pocketId);

        return Ok(fileUploadSummaries);
    }

    [HttpGet("pockets/{pocketId:guid}/folders/{folderId:guid}/files")]
    public async Task<IActionResult> GetAllFromPocket([FromRoute] Guid pocketId, [FromRoute] Guid folderId)
    {
        var fileUploadSummaries = await _service.FileService.GetAllFilesFromPocketAsync(pocketId, folderId);

        return Ok(fileUploadSummaries);
    }

    [HttpGet("files/filtered")]
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

    //[HttpGet("check")]
    //public async Task<IActionResult> CheckIfFileExists(string fileName, Guid storageId)
    //{
    //    var response = await _service.FileService.CheckIfFileExists(fileName, storageId);

    //    return Ok(response);
    //}

    [HttpGet("pockets/{pocketId:guid}/files/{fileId:guid}")]
    public async Task<IActionResult> Get(Guid pocketId, Guid fileId)
    {
        var file = await _service.FileService.GetFileByIdAsync(pocketId, fileId);

        return Ok(file);
    }

    [HttpGet("pockets/{pocketId:guid}/files/{fileId:guid}/info")]
    public async Task<IActionResult> GetInfo(Guid pocketId, Guid fileId)
    {
        var file = await _service.FileService.GetFileInfoByIdAsync(pocketId, fileId);

        return Ok(file);
    }

    [HttpGet("pockets/{pocketId:guid}/files/{imageId:guid}/thumbnail/{size}")]
    public async Task<IActionResult> GetImageThumbnail(
        [FromRoute, Required] Guid pocketId,
        [FromRoute, Required] Guid imageId,
        [FromRoute, Required] int size)
    {
        var image = await _service.FileService.GetThumbnailAsync(pocketId, imageId, size);

        return Ok(image);
    }

    [HttpPost("pockets/{pocketId:guid}/thumbnails/{size}")]
    public async Task<IActionResult> GetImageThumbnails(
        [FromBody, Required] Guid[] imageIds,
        [FromRoute, Required] Guid pocketId,
        [FromRoute, Required] int size)
    {
        var images = await _service.FileService.GetThumbnailsAsync(pocketId, imageIds,  size);

        return CreatedAtRoute("All", new { pocketId }, images!);
    }


    #endregion

    #region POST
    [HttpPost("files")]
    public async Task<IActionResult> UploadFile([FromForm] FileInformation fileInformation)
    {
        var validationResult = ValidateFile(fileInformation);
        if (validationResult is not null)
        {
            return validationResult;
        }

        try
        {
            var fileUploadSummary = await _service.FileService.UploadFileAsync(fileInformation.File!, fileInformation.ClientId!.Value, fileInformation.PocketId!.Value, fileInformation.FolderId);

            return Ok(fileUploadSummary);
        }
        catch (FileAlreadyUploadedException e)
        {
            return BadRequest(e.Message);
        }
    }

    #endregion

    #region DELETE
    [HttpDelete("pockets/{pocketId:guid}/files/{fileId:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid pocketId, [FromRoute] Guid fileId)
    {
        await _service.FileService.DeleteFileAsync(pocketId, fileId);

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

        if (fileInformation.PocketId is null)
        {
            return BadRequest("StorageId cannot be null");
        }

        return null;
    }

    public class FileInformation
    {
        public Guid? ClientId { get; set; }
        public Guid? PocketId { get; set; }
        public Guid? FolderId { get; set; }
        public IFormFile? File { get; set; }
    }
}
