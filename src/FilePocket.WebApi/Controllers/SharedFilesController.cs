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
    }
}
