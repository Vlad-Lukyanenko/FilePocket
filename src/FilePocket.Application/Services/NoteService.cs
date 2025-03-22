using AutoMapper;
using FilePocket.Application.Exceptions;
using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using MongoDB.Driver;


namespace FilePocket.Application.Services
{
    public class NoteService : INoteService
    {

        private readonly INotesRepository _notes;
        private readonly IMapper _mapper;

        public NoteService(INotesRepository notes, IMapper mapper)
        {
            _notes = notes;
            _mapper = mapper;
        }

        public async Task<NoteCreateResponse> CreateAsync(NoteCreateModel note, CancellationToken cancellationToken = default)
        {

            var noteEntity = new Note
            {
                UserId = note.UserId,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _notes.CreateAsync(noteEntity, cancellationToken);

            return _mapper.Map<NoteCreateResponse>(noteEntity);
        }

        public async Task<NoteUpdateResponse> UpdateAsync(NoteModel note, CancellationToken cancellationToken = default)
        {
            var noteEntity = await GetNoteIfExists(note.Id, cancellationToken);

            _mapper.Map(note, noteEntity);
            noteEntity.UpdatedAt = DateTime.UtcNow;

            await _notes.UpdateAsync(noteEntity, cancellationToken);

            return _mapper.Map<NoteUpdateResponse>(noteEntity);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _ = await GetNoteIfExists(id, cancellationToken);

            await _notes.DeleteAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<NoteModel>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var notes = await _notes.GetAllByUserIdAsync(userId, cancellationToken)
                ?? [];

            return _mapper.Map<IEnumerable<NoteModel>>(notes);
        }

        public async Task<NoteModel> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var note = await GetNoteIfExists(id, cancellationToken);

            return _mapper.Map<NoteModel>(note);
        }

        public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _ = await GetNoteIfExists(id, cancellationToken);

            await _notes.SoftDeleteAsync(id, cancellationToken);
        }

        private async Task<Note> GetNoteIfExists(Guid id, CancellationToken cancellationToken)
        {
            var noteToProcess = await _notes.GetByIdAsync(id, cancellationToken)
               ?? throw new NoteNotFoundException(id);

            return noteToProcess;
        }
    }
}
