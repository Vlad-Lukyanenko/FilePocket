using FilePocket.Contracts.Services;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace FilePocket.WebApi.Controllers;

[Route("api/token")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly IServiceManager _service;

    public TokenController(IServiceManager service)
    {
        _service = service;
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenModel tokenModel)
    {
        if (tokenModel is null)
        {
            return BadRequest("User to login is null");
        }

        var tokenDtoToReturn = await _service.AuthenticationService.RefreshToken(tokenModel);

        return Ok(tokenDtoToReturn);
    }
}
