using FilePocket.Contracts.Services;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace FilePocket.WebApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private readonly IServiceManager _service;

        public FoldersController(IServiceManager service)
        {
            _service = service;
        }

        [HttpPost("folders")]
        public async Task<IActionResult> Create([FromBody] FolderModel folder)
        {
            await _service.FolderService.CreateAsync(folder);

            return Ok();
        }

        [HttpGet("folders/{folderId:guid}")]
        public async Task<IActionResult> Get([FromRoute] Guid folderId)
        {
            var folder = await _service.FolderService.GetAsync(folderId);

            return Ok(folder);
        }

        [HttpGet("pockets/{pocketId:guid}/folders")]
        public async Task<IActionResult> GetAll([FromRoute] Guid pocketId)
        {
            var folders = await _service.FolderService.GetAllAsync(pocketId, null);

            return Ok(folders);
        }

        [HttpGet("pockets/{pocketId:guid}/parent-folder/{parentFolderId:guid}/folders")]
        public async Task<IActionResult> GetAll([FromRoute] Guid pocketId, [FromRoute] Guid? parentFolderId)
        {
            var folders = await _service.FolderService.GetAllAsync(pocketId, parentFolderId);

            return Ok(folders);
        }

        [HttpDelete("folders/{folderId:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid folderId)
        {
            await _service.FolderService.DeleteAsync(folderId);

            return Ok();
        }
    }
}
