using FilePocket.Contracts.Services;
using FilePocket.Domain.Models;
using FilePocket.Domain.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FilePocket.WebApi.Controllers
{
    [ApiController]
    [Route("api/clients/")]
    [ServiceFilter(typeof(RequireHeaderAttribute))]
    public class ClientAppsRequestsController : ControllerBase
    {
        private readonly IServiceManager _service;

        public ClientAppsRequestsController(IServiceManager service)
        {
            _service = service;
        }

        [HttpPost("pockets")]
        public async Task<IActionResult> Create([FromBody] StorageForManipulationsModel pocket)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var createdStorage = await _service.StorageService.CreateStorageAsync(pocket);

            return Ok(createdStorage);
        }
    }
}
