﻿using FilePocket.Domain.Entities;

namespace FilePocket.Contracts.Repositories;

public interface IFileMetadataRepository
{
    Task<List<FileMetadata>> GetRecentFilesAsync(Guid userId, int numberOfFiles);

    Task<List<FileMetadata>> GetAllAsync(Guid pocketId, bool trackChanges = false);

    Task<List<FileMetadata>> GetAllAsync(Guid userId, Guid pocketId, Guid? folderId, bool trackChanges = false);

    Task<FileMetadata> GetByIdAsync(Guid userId, Guid fileId, bool trackChanges = false);

    void UpdateFileMetadata(FileMetadata fileMetadata);

    void CreateFileMetadata(FileMetadata fileMetadata);

    void DeleteFileMetadata(FileMetadata fileMetadata);
}
