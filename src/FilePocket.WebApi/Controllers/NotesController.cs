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
        private readonly IServiceManager _service;

        public NotesController(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet("notes")]
        [HttpGet("folders/{folderId:guid}/notes")]
        public async Task<IActionResult> GetAllByUserIdAndFolderId(Guid? folderId)
        {
            var notes = await _service.FileService.GetAllNotesMetadataAsync(UserId, folderId, false);

            return Ok(notes);
        }

        [HttpGet("notes/{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var note = await _service.FileService.GetNoteByUserIdAndIdAsync(UserId, id);

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

            var result = await _service.FileService.CreateNoteContentFileAsync(note);

            return Ok(result);
        }

        [HttpPut("notes")]
        public async Task<IActionResult> Update([FromBody] NoteModel note)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var result = await _service.FileService.UpdateNoteContentFileAsync(note);

            return Ok(result);
        }

        [HttpDelete("notes/{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            _ = await _service.FileService.MoveToTrash(UserId, id);

            return NoContent();
        }

        [HttpDelete("notes/{id:guid}/delete-irreversibly")]
        public async Task<IActionResult> IrreversiblyDelete([FromRoute] Guid id)
        {
            _ = await _service.FileService.RemoveFileAsync(UserId, id);

            return NoContent();
        }
    }
}
