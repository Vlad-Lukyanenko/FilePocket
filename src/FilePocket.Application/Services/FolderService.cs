using AutoMapper;
using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;

namespace FilePocket.Application.Services
{
    public class FolderService : IFolderService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public FolderService(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<FolderModel> CreateAsync(FolderModel folder)
        {
            var folderExists = await _repository.Folder.ExistsAsync(folder.Name, folder.PocketId, folder.ParentFolderId);
            if (folderExists)
            {
                throw new InvalidOperationException("A folder with the same name already exists.");
            }
            var folderEntity = _mapper.Map<Folder>(folder);

            _repository.Folder.Create(folderEntity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<FolderModel>(folderEntity);

        }

        public async Task<FolderModel?> GetAsync(Guid folderId)
        {
            var folder = await _repository.Folder.GetAsync(folderId);

            return _mapper.Map<FolderModel?>(folder);
        }

        public async Task DeleteAsync(Guid folderId)
        {
            await _repository.Folder.Delete(folderId);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteByPocketIdAsync(Guid pocketId)
        {
            _repository.Folder.DeleteByPocketId(pocketId);
            await _repository.SaveChangesAsync();
        }

        public async Task MoveToTrash(Guid userId, Guid folderId)
        {
            var folder = await _repository.Folder.GetAsync(folderId);

            if (folder is null)
                return;

            folder.IsDeleted = true;
            folder.DeletedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();
        }

        public async Task<List<FolderModel>> GetAllAsync(Guid userId, Guid? pocketId, Guid? parentFolderId)
        {
            var result = await _repository.Folder.GetAllAsync(userId, pocketId, parentFolderId);

            return _mapper.Map<List<FolderModel>>(result);
        }
    }
}
