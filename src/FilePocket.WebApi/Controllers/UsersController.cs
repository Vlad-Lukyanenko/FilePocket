using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FilePocket.WebApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetUserByName(string name)
        {
            var user = await _userManager.FindByNameAsync(name);

            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequest updateUserRequest)
        {
            var user = await _userManager.FindByNameAsync(updateUserRequest.UserName);

            if (user is not null)
            {
                user.WithFirstName(updateUserRequest.FirstName!)
                    .WithLastName(updateUserRequest.LastName!);

                await _userManager.UpdateAsync(user);
            }
            
            return Ok();
        }
    }
}
