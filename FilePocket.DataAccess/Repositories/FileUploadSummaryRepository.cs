using FilePocket.Contracts.Repositories;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FilePocket.DataAccess.Repositories;

public class FileUploadSummaryRepository : RepositoryBase<FileUploadSummary>, IFileUploadSummaryRepository
{
    public FileUploadSummaryRepository(FilePocketDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<FileUploadSummary>> GetAllByStorageIdAsync(Guid storageId, bool trackChanges)
    {
        return await FindByCondition(e => e.StorageId.Equals(storageId), trackChanges).ToListAsync();
    }

    public async Task<IEnumerable<FileUploadSummary>> GetFilteredFilesAsync(FilesFilterOptionsModel filterOptionsModel, bool trackChanges)
    {
        var files = await FindByCondition(f =>
            (filterOptionsModel.StorageId == null
            ? f.Storage!.UserId.Equals(filterOptionsModel.UserId)
            : f.StorageId.Equals(filterOptionsModel.StorageId))
                && (filterOptionsModel.AfterDate == null || f.DateCreated >= filterOptionsModel.AfterDate)
                && (filterOptionsModel.BeforeDate == null || f.DateCreated < filterOptionsModel.BeforeDate.Value.AddDays(1))
                && (string.IsNullOrEmpty(filterOptionsModel.FileType) || f.FileType == filterOptionsModel.FileType)
                && (string.IsNullOrEmpty(filterOptionsModel.OriginalNameContains) || f.OriginalName!.Contains(filterOptionsModel.OriginalNameContains)),
                trackChanges)
            .OrderBy(f => f.Id)
            .Skip(filterOptionsModel.PageSize * (filterOptionsModel.PageNumber - 1))
            .Take(filterOptionsModel.PageSize)
            .ToListAsync();

        return files;
    }

    public async Task<int> GetFilteredCountAsync(FilesFilterOptionsModel filterOptionsModel, bool trackChanges)
    {
        var count = await FindByCondition(f =>
        (filterOptionsModel.StorageId == null
        ? f.Storage!.UserId.Equals(filterOptionsModel.UserId)
        : f.StorageId.Equals(filterOptionsModel.StorageId))
            && (filterOptionsModel.AfterDate == null || f.DateCreated >= filterOptionsModel.AfterDate)
            && (filterOptionsModel.BeforeDate == null || f.DateCreated < filterOptionsModel.BeforeDate.Value.AddDays(1))
            && (string.IsNullOrEmpty(filterOptionsModel.FileType) || f.FileType == filterOptionsModel.FileType)
            && (string.IsNullOrEmpty(filterOptionsModel.OriginalNameContains) || f.OriginalName!.Contains(filterOptionsModel.OriginalNameContains)),
            trackChanges)
        .CountAsync();

        return count;
    }

    public async Task<FileUploadSummary> GetByIdAsync(Guid storageId, Guid fileUploadSummaryId, bool trackChanges)
    {
        return (await FindByCondition(us => us.StorageId.Equals(storageId) && us.Id.Equals(fileUploadSummaryId), trackChanges).SingleOrDefaultAsync())!;
    }

    public void CreateFileUploadSummary(FileUploadSummary uploadSummary)
    {
        Create(uploadSummary);
    }

    public void DeleteFileUploadSummary(FileUploadSummary uploadSummary)
    {
        Delete(uploadSummary);
    }

    public async Task<bool> CheckIfFileExists(string fileName, string fileType, Guid storageId, bool trackChanges)
    {
        return await FindByCondition(f => 
            f.StorageId == storageId
            && f.FileType == fileType
            && f.OriginalName == fileName,
            trackChanges).AnyAsync();
    }

    public void UpdateFileUploadSummary(FileUploadSummary uploadSummary)
    {
        Update(uploadSummary);
    }
}
