using FilePocket.Contracts.Repositories;
using FilePocket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FilePocket.DataAccess.Repositories
{
    public class FolderRepository : IFolderRepository
    {
        protected FilePocketDbContext DbContext;
        private readonly IServiceScopeFactory _scopeFactory;

        public FolderRepository(FilePocketDbContext repositoryContext, IServiceScopeFactory scopeFactory)
        {
            DbContext = repositoryContext;
            _scopeFactory = scopeFactory;
        }

        public void Create(Folder folder)
        {
            DbContext.Set<Folder>().Add(folder);
        }

        public async Task Delete(Guid folderId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FilePocketDbContext>();

                using (var transaction = await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await DeleteFolderIterative(folderId, dbContext);
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public void DeleteByPocketId(Guid pocketId)
        {
            var folders = DbContext.Set<Folder>().Where(x => x.PocketId == pocketId);

            if (folders.Any())
            {
                DbContext.Set<Folder>().RemoveRange(folders);
            }
        }

        public async Task<List<Folder>> GetAllAsync(Guid userId, Guid? pocketId, Guid? parentFolderId)
        {
            var result = DbContext.Set<Folder>().Where(c => c.UserId == userId && c.PocketId == pocketId && c.ParentFolderId == parentFolderId);

            return await result.ToListAsync();
        }

        public Task<Folder?> GetAsync(Guid folderId)
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
    }
}
