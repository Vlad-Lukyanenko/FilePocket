using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Enums;
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

        var createdPocket = await _service.PocketService.CreatePocketAsync(defaultPocket);
        await CreateProfileAsync(result.User);
        await CreateDefaultFoldersAsync(createdPocket.Id, createdPocket.UserId)
;
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

    private async Task CreateDefaultFoldersAsync(Guid defaultPocketId, Guid userId)
    {
        var defaultFolders = new List<FolderModel>
        {
            new FolderModel() { UserId = userId, PocketId = defaultPocketId, Name = "Documents", FolderType = FolderType.Documents },
            new FolderModel() { UserId = userId, PocketId = defaultPocketId, Name = "Music", FolderType = FolderType.Files  },
            new FolderModel() { UserId = userId, PocketId = defaultPocketId, Name = "Books", FolderType = FolderType.Files  },
            new FolderModel() { UserId = userId, PocketId = defaultPocketId, Name = "Pictures", FolderType = FolderType.Files  },
            new FolderModel() { UserId = userId, PocketId = defaultPocketId, Name = "Videos", FolderType = FolderType.Files  },
            new FolderModel() { UserId = userId, PocketId = defaultPocketId, Name = "Other", FolderType = FolderType.Files  }
        };

        foreach (var folder in defaultFolders)
        {
            await _service.FolderService.CreateAsync(folder);
        }
    }
}
