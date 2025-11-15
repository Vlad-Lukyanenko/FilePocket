using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Entities.Abstractions;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FilePocket.Infrastructure.Persistence.Repositories
{
    public class SharedFileRepository : RepositoryBase<SharedFile>, ISharedFileRepository
    {
        private readonly UserManager<User> _userManager;

        public SharedFileRepository(FilePocketDbContext context, UserManager<User> userManager)
        : base(context)
        {
            _userManager = userManager;
        }

        public async Task<SharedFileModel?> GetAggregatedDataByIdAsync(Guid sharedFileId)
        {
            var sharedFile = await DbContext.SharedFiles
                            .Where(sf => sf.Id == sharedFileId)
                            .Join(DbContext.FilesMetadata,
                                  sf => sf.FileId,
                                  fl => fl.Id,
                                  (sf, fl) => new { sf, fl })
                            .Join(_userManager.Users,
                                  x => x.sf.UserId,
                                  usr => usr.Id,
                                  (x, usr) => new SharedFileModel
                                  {
                                      Id = x.sf.Id,
                                      FileId = x.sf.FileId,
                                      PocketId = x.fl.PocketId,
                                      FileType = x.fl.FileType,
                                      CreatedAt = x.sf.CreatedAt,
                                      FileName = x.fl.OriginalName,
                                      FirstName = usr.FirstName,
                                      LastName = usr.LastName,
                                      FileSize = x.fl.FileSize
                                  })
                            .SingleOrDefaultAsync();

            return sharedFile;
        }

        public async Task<IBaseMetadata?> GetFileBaseMetadataAsync(Guid sharedFileId)
        {
            var sharedFile = await DbContext.SharedFiles
                            .Where(sf => sf.Id == sharedFileId)
                            .Join(DbContext.FilesMetadata,
                                  sf => sf.FileId,
                                  fl => fl.Id,
                                  (sf, fl) => new { sf, fl })
                            .Select(x => new FileBaseMetadata()
                            {
                                Id = x.fl.Id,
                                Path = x.fl.Path,
                                ActualName = x.fl.ActualName
                            })
                            .SingleOrDefaultAsync();

            return sharedFile;
        }

        public Task<SharedFile?> GetByIdAsync(Guid sharedFileId)
        {
            return FindByCondition(e => e.Id.Equals(sharedFileId));
        }

        public async Task<List<SharedFileView>> GetAllAsync(Guid userId, bool trackChanges)
        {
            var result = from sharedFile in DbContext.SharedFiles
                         join file in DbContext.FilesMetadata
                         on sharedFile.FileId equals file.Id
                         where sharedFile.UserId == userId
                         select new SharedFileView
                         {
                             SharedFileId = sharedFile.Id,
                             FileType = file.FileType,
                             OriginalName = file.OriginalName,
                             FileSize = file.FileSize,
                             CreatedAt = sharedFile.CreatedAt
                         };

            return await result.ToListAsync();
        }

        public async Task<List<SharedFileView>> GetLatestAsync(Guid userId, int number, bool trackChanges)
        {
            var result = from sharedFile in DbContext.SharedFiles
                         join file in DbContext.FilesMetadata
                         on sharedFile.FileId equals file.Id
                         where sharedFile.UserId == userId
                         select new SharedFileView
                         {
                             SharedFileId = sharedFile.Id,
                             FileType = file.FileType,
                             OriginalName = file.OriginalName,
                             FileSize = file.FileSize,
                             CreatedAt = sharedFile.CreatedAt
                         };

            return await result.OrderByDescending(c => c.CreatedAt).Take(number).ToListAsync();
        }
    }
}
