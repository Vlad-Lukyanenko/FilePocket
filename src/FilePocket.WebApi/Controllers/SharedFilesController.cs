using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilePocket.WebApi.Controllers
{
    [Route("api/files/shared")]
    [ApiController]
    [Authorize]
    public class SharedFilesController : BaseController
    {
        private readonly IServiceManager _service;

        public SharedFilesController(IServiceManager service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SharedFileModel sharedFile)
        {
            await _service.SharedFileService.CreateAsync(UserId, sharedFile);

            return Ok();
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var sharedFiles = await _service.SharedFileService.GetAllAsync(UserId, trackChanges: false);

            return Ok(sharedFiles);
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest()
        {
            var latestSharedFiles = await _service.SharedFileService.GetLatestAsync(UserId, 10);

            return Ok(latestSharedFiles);
        }

        [AllowAnonymous]
        [HttpGet("{sharedFileId:guid}")]
        public async Task<IActionResult> GetSharedFile([FromRoute] Guid sharedFileId)
        {
            var sharedFile = await _service.SharedFileService.GetByIdAsync(sharedFileId);

            return Ok(sharedFile);
        }

        [AllowAnonymous]
        [HttpGet("download/{sharedFileId:guid}")]
        public async Task<IActionResult> Download([FromRoute] Guid sharedFileId)
        {
            var sharedFile = await _service.SharedFileService.DownloadFileAsync(sharedFileId);

            return Ok(sharedFile);
        }

        [HttpDelete("{sharedFileId:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid sharedFileId)
        {
            await _service.SharedFileService.Delete(sharedFileId);

            return NoContent();
        }
    }
}
