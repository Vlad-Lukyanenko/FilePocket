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
        private readonly IEncryptionService _encryptionService;
        private readonly IMapper _mapper;

        public NoteService(IRepositoryManager manager, IEncryptionService encryptionService, IMapper mapper)
        {
            _manager = manager;
            _encryptionService = encryptionService;
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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _manager.Note.CreateAsync(noteEntity, cancellationToken);

            if (!string.IsNullOrEmpty(note.Content))
            {
                var encryptedContent = await EncryptContent(note.UserId, noteEntity.Id, note.Content, cancellationToken);
                noteEntity.EncryptedContent = Convert.FromBase64String(encryptedContent);
            }

            await _manager.Note.AddEncryptedContent(noteEntity, cancellationToken);

            return _mapper.Map<NoteCreateResponse>(noteEntity);
        }

        public async Task<NoteUpdateResponse> UpdateAsync(NoteModel note, CancellationToken cancellationToken = default)
        {
            var noteEntity = await GetNoteIfExists(note.Id, cancellationToken);

            _mapper.Map(note, noteEntity);
            noteEntity.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(note.Content))
            {
                var encryptedContent = await EncryptContent(note.UserId, noteEntity.Id, note.Content, cancellationToken);
                noteEntity.EncryptedContent = Convert.FromBase64String(encryptedContent);
            }
            else if(noteEntity.EncryptedContent.Length != 0)
            {
                noteEntity.EncryptedContent = [];
            }

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
            var noteModel = _mapper.Map<NoteModel>(note);

            if (note.EncryptedContent != null && note.EncryptedContent.Length > 0)
            {
                var decryptedContent = await DecryptContent(note.UserId, note.Id, note.EncryptedContent, cancellationToken);
                noteModel.Content = decryptedContent;
            }

            return noteModel;
        }

        public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _ = await GetNoteIfExists(id, cancellationToken);

            await _manager.Note.SoftDeleteAsync(id, cancellationToken);
        }

        public async Task BulkDeleteAsync(Guid folderId, CancellationToken cancellationToken = default)
        {
            await _manager.Note.BulkDeleteAsync(folderId, cancellationToken);
        }

        public async Task BulkSoftDeleteAsync(Guid folderId, CancellationToken cancellationToken = default)
        {
            await _manager.Note.BulkSoftDeleteAsync(folderId, cancellationToken);
        }

        private async Task<Note> GetNoteIfExists(Guid id, CancellationToken cancellationToken)
        {
            var noteToProcess = await _manager.Note.GetByIdAsync(id, cancellationToken)
               ?? throw new NoteNotFoundException(id);

            return noteToProcess;
        }

        private async Task<string> EncryptContent(Guid userId, Guid noteId, string text, CancellationToken cancellationToken)
        {
            var passPhrase = _encryptionService.GeneratePassPhrase(userId, noteId);
            var noteEncryptedContent = await _encryptionService.EncryptAsync(text, passPhrase, cancellationToken);

            return Convert.ToBase64String(noteEncryptedContent);
        }

        private async Task<string> DecryptContent(Guid userId, Guid noteId, byte[] encryptedContent, CancellationToken cancellationToken)
        {
            var passPhrase = _encryptionService.GeneratePassPhrase(userId, noteId);
            var decryptedText = await _encryptionService.DecryptAsync(encryptedContent, passPhrase, cancellationToken);
            return decryptedText;
        }
    }
}
