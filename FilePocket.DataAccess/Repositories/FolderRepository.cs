using FilePocket.Contracts.Repositories;
using FilePocket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilePocket.DataAccess.Repositories
{
    public class FolderRepository : IFolderRepository
    {
        protected FilePocketDbContext DbContext;

        public FolderRepository(FilePocketDbContext repositoryContext)
        {
            DbContext = repositoryContext;
        }

        public void Create(Folder folder)
        {
            DbContext.Set<Folder>().Add(folder);
        }

        public void Delete(Guid folderId)
        {
            var folder = DbContext.Set<Folder>().FirstOrDefault(x => x.Id == folderId);

            if (folder is not null)
            {
                DbContext.Set<Folder>().Remove(folder);
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

        public async Task<List<Folder>> GetAllAsync(Guid pocketId, Guid? parentFolderId)
        {
            var result = DbContext.Set<Folder>().Where(c => c.PocketId == pocketId && c.ParentFolderId == parentFolderId);

            return await result.ToListAsync();
        }

        public Task<Folder?> GetAsync(Guid folderId)
        {
            return DbContext.Set<Folder>().FirstOrDefaultAsync(x => x.Id == folderId);
        }
    }
}
