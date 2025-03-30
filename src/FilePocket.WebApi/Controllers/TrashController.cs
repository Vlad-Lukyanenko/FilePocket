using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace FilePocket.WebApi.Controllers
{
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

        [HttpPut("folders/{folderId:guid}")]
        public async Task<IActionResult> MoveFolderToTrash([FromRoute] Guid folderId)
        {
            var folder = await _service.FolderService.GetAsync(folderId);

            if (folder?.FolderType == Domain.Enums.FolderType.Documents)
            {
                try
                {
                    await _service.NoteService.BulkSoftDeleteAsync(folderId);
                }
                catch
                {
                    return Problem("Error deleting folder content");
                }
            }

            await _service.FolderService.MoveToTrash(UserId, folderId);

            return Ok();
        }

        [HttpPut("pockets/{pocketId:guid}")]
        public async Task<IActionResult> MovePocketToTrash([FromRoute] Guid pocketId)
        {
            await _service.PocketService.MoveToTrash(UserId, pocketId);

            return Ok();
        }
    }
}
