using MapsterMapper;
using FilePocket.Application.Exceptions;
using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;


namespace FilePocket.Application.Services;
public class NoteService : INoteService
{
    private readonly IRepositoryManager _manager;
    private readonly IEncryptionService _encryptionService;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public NoteService(IRepositoryManager manager, IEncryptionService encryptionService, IFileService fileService, IMapper mapper)
    {
        _manager = manager;
        _encryptionService = encryptionService;
        _fileService = fileService;
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

        var encryptedContent = await EncryptContent(note.UserId, noteEntity.Id, note.Content, cancellationToken);
        var contentBytes = Convert.FromBase64String(encryptedContent);
        var writeContentResponse = await _fileService.CreateNoteContentToFileAsync(noteEntity, contentBytes, cancellationToken)
                                   ?? throw new ArgumentNullException();

        noteEntity.ContentFileMetadataId = writeContentResponse.Id;
        await _manager.Note.AddEncryptedContent(noteEntity, cancellationToken);

        return _mapper.Map<NoteCreateResponse>(noteEntity);
    }

    public async Task<NoteUpdateResponse> UpdateAsync(NoteModel note, CancellationToken cancellationToken = default)
    {
        var noteEntity = await GetNoteIfExists(note.Id, cancellationToken);

        _mapper.Map(note, noteEntity);
        noteEntity.UpdatedAt = DateTime.UtcNow;

        var encryptedContent = await EncryptContent(note.UserId, noteEntity.Id, note.Content, cancellationToken);
        var contentBytes = Convert.FromBase64String(encryptedContent);
        var writeContentResponse = await _fileService.UpdateNoteContentFileAsync(noteEntity, contentBytes, cancellationToken)
                                   ?? throw new ArgumentNullException();

        noteEntity.ContentFileMetadataId = writeContentResponse.Id;

        await _manager.Note.UpdateAsync(noteEntity, cancellationToken);

        return _mapper.Map<NoteUpdateResponse>(noteEntity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var noteEntity = await GetNoteIfExists(id, cancellationToken);
        var deleteResult = await _fileService.RemoveFileAsync(noteEntity.UserId, noteEntity.ContentFileMetadataId, cancellationToken);

        if (deleteResult)
        {
            await _manager.Note.DeleteAsync(id, cancellationToken);
        }
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

        if (note.ContentFileMetadataId != Guid.Empty)
        {
            var contentBytes = await _fileService.ReadNoteContentFromFileAsync(note.UserId, note.ContentFileMetadataId);
            var decryptedContent = await DecryptContent(note.UserId, note.Id, contentBytes, cancellationToken);

            noteModel.Content = decryptedContent;
        }

        return noteModel;
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var noteEntity = await GetNoteIfExists(id, cancellationToken);

        await _manager.Note.SoftDeleteAsync(id, cancellationToken);
        _ = await _fileService.MoveToTrash(noteEntity.UserId, noteEntity.ContentFileMetadataId, cancellationToken);
    }

    public async Task BulkDeleteAsync(Guid userId, Guid folderId, CancellationToken cancellationToken = default)
    {
        var notes = (await _manager.Note.GetAllByUserIdAndFolderIdAsync(userId, folderId, cancellationToken))
            .Where(n => n.IsDeleted && n.ContentFileMetadataId != Guid.Empty);

        var removeContentFileTasks = notes.Select(n => _fileService.RemoveFileAsync(userId, n.ContentFileMetadataId, cancellationToken));

        await Task.WhenAll(removeContentFileTasks);
        await _manager.Note.BulkDeleteAsync(folderId, cancellationToken);

    }

    public async Task BulkSoftDeleteAsync(Guid userId, Guid folderId, CancellationToken cancellationToken = default)
    {
        var notes = (await _manager.Note.GetAllByUserIdAndFolderIdAsync(userId, folderId, cancellationToken))
            .Where(n => !n.IsDeleted && n.ContentFileMetadataId != Guid.Empty);

        var moveToTrashTasks = notes.Select(n => _fileService.MoveToTrash(userId, n.ContentFileMetadataId, cancellationToken));

        await Task.WhenAll(moveToTrashTasks);
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
