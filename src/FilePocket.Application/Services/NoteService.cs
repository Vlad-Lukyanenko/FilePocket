using MapsterMapper;
using FilePocket.Application.Exceptions;
using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;


namespace FilePocket.Application.Services
{
    public class NoteService : INoteService
    {

        private readonly IRepositoryManager _manager;
        private readonly IMapper _mapper;

        public NoteService(IRepositoryManager manager, IMapper mapper)
        {
            _manager = manager;
            _mapper = mapper;
        }

        public async Task<NoteCreateResponse> CreateAsync(NoteCreateModel note, CancellationToken cancellationToken = default)
        {

            var noteEntity = new Note
            {
                UserId = note.UserId,
                PocketId = note.PocketId,
                FolderId = note.FolderId,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _manager.Note.CreateAsync(noteEntity, cancellationToken);

            return _mapper.Map<NoteCreateResponse>(noteEntity);
        }

        public async Task<NoteUpdateResponse> UpdateAsync(NoteModel note, CancellationToken cancellationToken = default)
        {
            var noteEntity = await GetNoteIfExists(note.Id, cancellationToken);

            _mapper.Map(note, noteEntity);
            noteEntity.UpdatedAt = DateTime.UtcNow;

            await _manager.Note.UpdateAsync(noteEntity, cancellationToken);

            return _mapper.Map<NoteUpdateResponse>(noteEntity);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _ = await GetNoteIfExists(id, cancellationToken);

            await _manager.Note.DeleteAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<NoteModel>> GetAllByUserIdAndFolderIdAsync(Guid userId, Guid? folderId = null, CancellationToken cancellationToken = default)
        {
            var notes = await _manager.Note.GetAllByUserIdAndFolderIdAsync(userId, folderId, cancellationToken)
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

            await _manager.Note.SoftDeleteAsync(id, cancellationToken);
        }

        private async Task<Note> GetNoteIfExists(Guid id, CancellationToken cancellationToken)
        {
            var noteToProcess = await _manager.Note.GetByIdAsync(id, cancellationToken)
               ?? throw new NoteNotFoundException(id);

            return noteToProcess;
        }

        public async Task BulkDeleteAsync(Guid folderId, CancellationToken cancellationToken = default)
        {
            await _manager.Note.BulkDeleteAsync(folderId,  cancellationToken);
        }

        public async Task BulkSoftDeleteAsync(Guid folderId, CancellationToken cancellationToken = default)
        {
            await _manager.Note.BulkSoftDeleteAsync(folderId, cancellationToken);
        }
    }
}
