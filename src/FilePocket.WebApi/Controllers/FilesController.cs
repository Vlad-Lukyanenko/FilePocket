using FilePocket.WebApi.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using FilePocket.Domain.Models;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Application.Exceptions;

namespace FilePocket.WebApi.Controllers;

[Route("api/")]
[ApiController]
[ServiceFilter(typeof(JwtOrApiKeyAuthorizeAttribute))]
public class FilesController : BaseController
{
    private readonly IServiceManager _service;

    public FilesController(IServiceManager service)
    {
        _service = service;
    }

    #region POST
    [HttpPost("files")]
    [ProducesResponseType<FileResponseModel>((int) HttpStatusCode.OK)]
    [ProducesResponseType<BadRequestObjectResult>((int) HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UploadFile([FromForm] FileInformation fileInformation)
    {
        var validationResult = ValidateFile(fileInformation);
        if (validationResult is not null)
            return validationResult;

        try
        {
            var fileMetadata = await _service.FileService.UploadFileAsync(
                UserId, fileInformation.File!, fileInformation.PocketId, fileInformation.FolderId);

            return Ok(fileMetadata);
        }
        catch (FileAlreadyUploadedException e)
        {
            return BadRequest(e.Message);
        }
    }
    #endregion

    #region GET
    [HttpGet("pockets/{pocketId:guid}/folders/{folderId:guid}/{isSoftDeleted:bool}/files")]
    [HttpGet("pockets/{pocketId:guid}/{isSoftDeleted:bool}/files")]
    public async Task<IActionResult> GetAll([FromRoute] Guid pocketId, [FromRoute] Guid? folderId, [FromRoute] bool isSoftDeleted)
    {
        var fileMetadata = await _service.FileService.GetAllFilesAsync(UserId, pocketId, folderId, isSoftDeleted);

        return Ok(fileMetadata);
    }

    [HttpGet("files/{fileId:guid}")]
    public async Task<IActionResult> Get(Guid fileId)
    {
        var file = await _service.FileService.GetFileByIdAsync(UserId, fileId);

        return Ok(file);
    }

    [HttpGet("files/{fileId:guid}/info")]
    public async Task<IActionResult> GetInfo(Guid fileId)
    {
        var file = await _service.FileService.GetFileInfoByIdAsync(UserId, fileId);

        return Ok(file);
    }

    [HttpGet("files/{imageId:guid}/thumbnail/{size}")]
    public async Task<IActionResult> GetImageThumbnail(
        [FromRoute, Required] Guid imageId,
        [FromRoute, Required] int size)
    {
        var image = await _service.FileService.GetThumbnailAsync(UserId, imageId, size);

        if(image.FileByteArray!.Length == 0) return BadRequest(image);

        return Ok(image);
    }

    [HttpPost("files/thumbnails/{size}")]
    public async Task<IActionResult> GetImageThumbnails(
        [FromBody, Required] Guid[] imageIds,
        [FromRoute, Required] Guid? pocketId,
        [FromRoute, Required] int size)
    {
        var images = await _service.FileService.GetThumbnailsAsync(UserId, imageIds,  size);

        return CreatedAtRoute("All", new { pocketId }, images!);
    }
    #endregion

    #region PUT
    [HttpPut("files")]
    public async Task<IActionResult> Update(UpdateFileModel file)
    {
        file.UserId = UserId;
        await _service.FileService.UpdateFileAsync(file);

        return NoContent();
    }
    #endregion

    #region DELETE
    [HttpDelete("files/{fileId:guid}")]
    public async Task<IActionResult> Delete( [FromRoute] Guid fileId)
    {
        await _service.FileService.RemoveFileAsync(UserId, fileId);

        return NoContent();
    }
    #endregion

    private BadRequestObjectResult? ValidateFile(FileInformation fileInformation)
    {
        if (fileInformation.File is null || fileInformation.File.Length == 0)
        {
            return BadRequest("Nothing to upload.");
        }

        return null;
    }

    public class FileInformation
    {
        public Guid PocketId { get; set; }
        public Guid? FolderId { get; set; }
        public IFormFile? File { get; set; }
    }
}