using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilePocket.WebApi.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class NotesController : BaseController
    {
        private readonly INoteService _service;

        public NotesController(INoteService service)
        {
            _service = service;
        }

        [HttpGet("notes")]
        [HttpGet("folders/{folderId:guid}/notes")]
        public async Task<IActionResult> GetAllByUserIdAndFolderId(Guid? folderId)
        {
            var notes = await _service.GetAllByUserIdAndFolderIdAsync(UserId, folderId);

            return Ok(notes);
        }

        [HttpGet("notes/{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var note = await _service.GetByIdAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            return Ok(note);
        }

        [HttpPost("notes")]
        public async Task<IActionResult> Create([FromBody] NoteCreateModel note)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var result = await _service.CreateAsync(note);

            return Ok(result);
        }

        [HttpPut("notes")]
        public async Task<IActionResult> Update([FromBody] NoteModel note)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var result = await _service.UpdateAsync(note);

            return Ok(result);
        }

        [HttpDelete("notes/{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _service.SoftDeleteAsync(id);

            return NoContent();
        }

        [HttpDelete("notes/{id:guid}/delete-irreversibly")]
        public async Task<IActionResult> IrreversiblyDelete([FromRoute] Guid id)
        {
            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}
