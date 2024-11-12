using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;

namespace FilePocket.Contracts.Repositories;

public interface IFileUploadSummaryRepository
{
    Task<IEnumerable<FileUploadSummary>> GetAllByStorageIdAsync(Guid storageId, bool trackChanges = false);

    Task<IEnumerable<FileUploadSummary>> GetAllByStorageIdAndFolderIdAsync(Guid storageId, Guid folderId, bool trackChanges = false);

    Task<IEnumerable<FileUploadSummary>> GetFilteredFilesAsync(FilesFilterOptionsModel filterOptionsModel, bool trackChanges = false);

    Task<int> GetFilteredCountAsync(FilesFilterOptionsModel filterOptionsModel, bool trackChanges = false);

    Task<FileUploadSummary> GetByIdAsync(Guid storageId, Guid fileUploadSummaryId, bool trackChanges = false);

    void UpdateFileUploadSummary(FileUploadSummary uploadSummary);

    void CreateFileUploadSummary(FileUploadSummary uploadSummary);

    void DeleteFileUploadSummary(FileUploadSummary uploadSummary);

    Task<bool> CheckIfFileExists(string fileName, string fileType, Guid storageId, bool trackChanges = false);

}
