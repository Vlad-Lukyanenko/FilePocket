﻿using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FilePocket.Infrastructure.Persistence.Repositories;

public class FolderRepository : RepositoryBase<Folder>, IFolderRepository
{
    private readonly IServiceScopeFactory _scopeFactory;

    public FolderRepository(FilePocketDbContext repositoryContext, IServiceScopeFactory scopeFactory)
        : base(repositoryContext)
    {
        DbContext = repositoryContext;
        _scopeFactory = scopeFactory;
    }

    public IEnumerable<Folder> GetChildFolders(Guid parentFolderId, bool trackChanges = false)
    {
        return FindByCondition(f => f.ParentFolderId == parentFolderId, trackChanges);
    }

    public IEnumerable<Folder> GetAll(Guid userId, bool isSoftDeleted, bool trackChanges)
    {
        return FindByCondition(b => b.UserId == userId && b.IsDeleted == isSoftDeleted, trackChanges).OrderByDescending(b => b.CreatedAt);
    }

    public new void Create(Folder folder)
    {
        DbContext.Set<Folder>().Add(folder);
    }

    public new void Update(Folder folder)
    {
        DbContext.Set<Folder>().Update(folder);
    }

    public async Task Delete(Guid folderId)
    {

        var folderToDelete = await DbContext.Set<Folder>().FindAsync(folderId);

        if (folderToDelete != null)
        {
            DbContext.Set<Folder>().Remove(folderToDelete);
            await DbContext.SaveChangesAsync();
        }

        //using (var scope = _scopeFactory.CreateScope())
        //{
        //    var dbContext = scope.ServiceProvider.GetRequiredService<FilePocketDbContext>();

        //    using (var transaction = await dbContext.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            await DeleteFolderIterative(folderId, dbContext);
        //            await transaction.CommitAsync();
        //        }
        //        catch
        //        {
        //            await transaction.RollbackAsync();
        //            throw;
        //        }
        //    }
        //}
    }

    public void DeleteByPocketId(Guid pocketId)
    {
        var folders = DbContext.Set<Folder>().Where(x => x.PocketId == pocketId);

        if (folders.Any())
        {
            DbContext.Set<Folder>().RemoveRange(folders);
        }
    }

    public async Task<List<Folder>> GetAllAsync(Guid userId, Guid? pocketId, Guid? parentFolderId, List<FolderType> folderTypes, bool isSoftDeleted)
    {
        var result = DbContext.Set<Folder>().Where(c => c.UserId == userId
                                                        && c.PocketId == pocketId
                                                        && c.ParentFolderId == parentFolderId
                                                        && folderTypes.Contains(c.FolderType)
                                                        && c.IsDeleted == isSoftDeleted);

        return await result.ToListAsync();
    }

    public void DeleteFolders(IEnumerable<Folder> folders)
    {
        DeleteAll(folders);
    }

    public Task<Folder?> GetByIdAsync(Guid folderId)
    {
        return DbContext.Set<Folder>().FirstOrDefaultAsync(x => x.Id == folderId);
    }

    private async Task DeleteFolderIterative(Guid folderId, FilePocketDbContext dbContext)
    {

        var stack = new Stack<Guid>();
        stack.Push(folderId);

        while (stack.Count > 0)
        {
            var currentFolderId = stack.Pop();
            var files = await dbContext.FilesMetadata.Where(c => c.FolderId == currentFolderId).ToListAsync();
            var childFolders = await dbContext.Folders.Where(f => f.ParentFolderId == currentFolderId).ToListAsync();

            foreach (var childFolder in childFolders)
            {
                stack.Push(childFolder.Id);
            }

            var folderToDelete = await dbContext.Folders.FindAsync(currentFolderId);
            if (folderToDelete != null)
            {
                foreach (var file in files)
                {
                    //file.MarkAsDeleted();
                }

                dbContext.Folders.Remove(folderToDelete);
                await dbContext.SaveChangesAsync();
            }
        }
    }
    public async Task<bool> ExistsAsync(string folderName, Guid? pocketId, Guid? parentFolderId, FolderType folderType)
    {
        return await DbContext.Folders.AsNoTracking().AnyAsync(f => f.Name == folderName
        && f.PocketId == pocketId
        && f.ParentFolderId == parentFolderId
        && f.FolderType == folderType);
    }

    public async Task<List<Folder>> GetFoldersByPartialNameAsync(Guid userId, string partialName, bool trackChanges = false)
    {
        return await FindByCondition(f => f.UserId.Equals(userId) && f.Name.ToLower().Contains(partialName.ToLower()), trackChanges)
            .OrderBy(f => f.FolderType)
            .ToListAsync();
    }

    public async Task<List<Folder>> GetAllSoftDeletedAsync(Guid userId, bool trackChanges)
    {
        return await FindByCondition(f => f.UserId.Equals(userId) && f.IsDeleted, trackChanges)
            .ToListAsync();
    }
}
