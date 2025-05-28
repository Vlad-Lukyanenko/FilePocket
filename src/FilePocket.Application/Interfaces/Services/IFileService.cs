using FilePocket.Application.Exceptions;
using FilePocket.Application.Extensions;
using FilePocket.Domain;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace FilePocket.Application.Interfaces.Services;

// Handles read concerns
public interface IFileProvider
{
    Task<IEnumerable<FileResponseModel>> GetAllFilesMetadataAsync(
        Guid userId,
        Guid pocketId,
        Guid? folderId,
        bool isSoftDeleted);
    Task<IEnumerable<FileResponseModel>> GetAllFilesWithSoftDeletedAsync(
        Guid userId,
        Guid pocketId);

    Task<IEnumerable<NoteModel>> GetAllNotesMetadataAsync(
        Guid userId,
        Guid? folderId,
        bool isSoftDeleted);

    Task<FileResponseModel> GetFileByUserIdIdAsync(
        Guid userId,
        Guid fileId);

    Task<FileResponseModel> GetFileMetadataByUserIdAndIdAsync(
        Guid userId,
        Guid fileId);

    Task<NoteModel> GetNoteByUserIdAndIdAsync(
        Guid userId,
        Guid fileId);

    Task<List<FileResponseModel>> GetLatestAsync(
        Guid userId,
        int number);

    Task<FileResponseModel> GetThumbnailAsync(
        Guid userId,
        Guid fileId,
        int maxSize);

    Task<List<FileResponseModel>> GetThumbnailsAsync(
        Guid userId,
        Guid[] fileIds,
        int maxSize);
}

// Handles write concerns (and read concerns for backward compatibility right now)
public interface IFileService : IFileProvider
{
    Task<FileResponseModel?> UploadFileAsync(
        Guid userId,
        IFormFile file,
        Guid pocketId,
        Guid? folderId,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveFileAsync(
        Guid userId,
        Guid fileId,
        CancellationToken cancellationToken = default);

    Task<bool> MoveToTrash(
        Guid userId,
        Guid fileId,
        CancellationToken cancellationToken = default);

    Task UpdateFileAsync(UpdateFileModel file);

    Task<FileResponseModel?> CreateNoteContentFileAsync(
        NoteCreateModel note,
        CancellationToken cancellationToken = default);

    Task<FileResponseModel?> UpdateNoteContentFileAsync(
        NoteModel note,
        CancellationToken cancellationToken = default);

    Task<byte[]> ReadNoteContentFromFileAsync(Guid userId, Guid fileId);

    Task<IEnumerable<FileSearchResponseModel>> SearchAsync(Guid userId, string partialName);
}