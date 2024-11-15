using FilePocket.Contracts.Services;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace FilePocket.WebApi.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IServiceManager _service;
    public AuthenticationController(IServiceManager service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationModel userForRegistration)
    {
        if (userForRegistration is null || !ModelState.IsValid)
        {
            return BadRequest();
        }

        var result = await _service.AuthenticationService.RegisterUser(userForRegistration);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }

            return BadRequest(ModelState);
        }

        return StatusCode(201);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginModel? user)
    {
        if (user is null)
        {
            return BadRequest("User to login is null");
        }

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        if (!await _service.AuthenticationService.ValidateUser(user))
        {
            return Unauthorized();
        }

        var tokenModel = await _service.AuthenticationService.CreateToken(populateExp: true);

        return Ok(tokenModel);
    }
}
