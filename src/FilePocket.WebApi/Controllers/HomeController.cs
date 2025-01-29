using FilePocket.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilePocket.WebApi.Controllers
{
    [Route("api/home/")]
    [ApiController]
    [Authorize]
    public class HomeController: BaseController
    {
        private readonly IServiceManager _service;

        public HomeController(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet("recent-files")]
        public async Task<IActionResult> GetRecentlyUploadedFiles()
        {
            var recentFiles = await _service.FileService.GetRecentFiles(UserId, 10);

            return Ok(recentFiles);
        }

        [HttpGet("recent-files/shared")]
        public async Task<IActionResult> GetRecentlySharedFiles()
        {
            var recentFiles = await _service.SharedFileService.GetLatestAsync(UserId, 10);

            return Ok(recentFiles);
        }
    }
}
