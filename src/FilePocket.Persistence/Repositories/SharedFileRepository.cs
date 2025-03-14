using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FilePocket.Persistence.Repositories
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

        public async Task<DownloadFileModel?> GetFileBodyAsync(Guid sharedFileId)
        {
            var sharedFile = await DbContext.SharedFiles
                            .Where(sf => sf.Id == sharedFileId)
                            .Join(DbContext.FilesMetadata,
                                  sf => sf.FileId,
                                  fl => fl.Id,
                                  (sf, fl) => new { sf, fl })
                            .Select(x => new
                            {
                                x.fl.Path,
                                x.fl.ActualName
                            })
                            .SingleOrDefaultAsync();

            var fullPath = sharedFile!.Path != null
                            ? Path.Combine(sharedFile.Path, sharedFile.ActualName)
                            : string.Empty;

            var fileByteArray = await File.ReadAllBytesAsync(fullPath);

            return new DownloadFileModel()
            {
                File = fileByteArray
            };
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
