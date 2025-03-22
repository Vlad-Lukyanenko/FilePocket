using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Entities;
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

        if (!result.IdentityResult!.Succeeded)
        {
            foreach (var error in result.IdentityResult!.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }

            return BadRequest(ModelState);
        }

        var defaultPocket = new PocketForManipulationsModel
        {
            UserId = result.User!.Id,
            Name = string.Empty,
            IsDefault = true
        };

        await _service.PocketService.CreatePocketAsync(defaultPocket);
        await CreateProfileAsync(result.User);

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

    private async Task CreateProfileAsync(User registeredUser)
    {
        var newProfile = new ProfileModel
        {
            Email = registeredUser.Email,
            CreatedAt = DateTime.UtcNow,
            UserId = registeredUser.Id
        };

        await _service.ProfileService.CreateProfileAsync(newProfile);
    }
}
