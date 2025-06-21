using FilePocket.Application.Exceptions;
using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Enums;
using FilePocket.Domain.Models;
using MapsterMapper;

namespace FilePocket.Application.Services;

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
        var folderExists = await _repository.Folder.ExistsAsync(folder.Name, folder.PocketId, folder.ParentFolderId, folder.FolderType);
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
        var folder = await GetFolderAndCheckIfItExistsAsync(folderId);

        return _mapper.Map<FolderModel?>(folder);
    }

    public async Task DeleteAsync(Guid folderId)
    {
        var folderToDelete = await GetFolderAndCheckIfItExistsAsync(folderId);

        await _repository.Folder.Delete(folderToDelete.Id);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteByPocketIdAsync(Guid pocketId)
    {
        _repository.Folder.DeleteByPocketId(pocketId);
        await _repository.SaveChangesAsync();
    }

    public async Task MoveToTrashAsync(Guid folderId)
    {
        var folder = await GetFolderAndCheckIfItExistsAsync(folderId);
        await IterateAndMarkAsDeletedOrRestoredThroughChildFoldersAsync(folder, isDeleted: true);

        folder.MarkAsDeleted();
        folder.ParentFolderId = null;

        await _repository.SaveChangesAsync();
    }

    public async Task RestoreFromTrashAsync(Guid folderId)
    {
        var folder = await GetFolderAndCheckIfItExistsAsync(folderId);
        await IterateAndMarkAsDeletedOrRestoredThroughChildFoldersAsync(folder, isDeleted: false);

        folder.RestoreFromDeleted();
        folder.ParentFolderId = null;

        await _repository.SaveChangesAsync();
    }

    public async Task<List<FolderModel>> GetAllAsync(Guid userId, Guid? pocketId, Guid? parentFolderId, List<FolderType> folderTypes, bool isSoftDeleted)
    {
        var result = await _repository.Folder.GetAllAsync(userId, pocketId, parentFolderId, folderTypes, isSoftDeleted);

        return _mapper.Map<List<FolderModel>>(result);
    }

    public async Task DeleteAllFoldersAsync(Guid userId)
    {
        var folders = _repository.Folder.GetAll(userId, true, false);

        _repository.Folder.DeleteFolders(folders);
        await _repository.SaveChangesAsync();
    }

    private async Task<Folder> GetFolderAndCheckIfItExistsAsync(Guid id)
    {
        var folder = await _repository.Folder.GetByIdAsync(id);

        if (folder is null)
        {
            throw new FolderNotFoundException(id);
        }

        return folder;
    }

    private async Task IterateAndMarkAsDeletedOrRestoredThroughChildFoldersAsync(Folder folder, bool isDeleted)
    {
        var stack = new Stack<Guid>();
        stack.Push(folder.Id);

        while (stack.Count > 0)
        {
            var currentFolderId = stack.Pop();
            var childFolders = _repository.Folder.GetChildFolders(currentFolderId, true).Where(f => f.IsDeleted == folder.IsDeleted);

            if (childFolders is not null && childFolders.Any())
            {
                foreach (var childFolder in childFolders)
                {
                    stack.Push(childFolder.Id);
                }
            }

            var folderToUpdate = await GetFolderAndCheckIfItExistsAsync(currentFolderId);

            if (isDeleted)
            {
                folderToUpdate.MarkAsDeleted();
            }
            else
            {
                folderToUpdate.RestoreFromDeleted();
            }
        }
    }

    public async Task<IEnumerable<FolderSearchResponseModel>> SearchAsync(Guid userId, string partialName)
    {
        var folders = await _repository.Folder.GetFoldersByPartialNameAsync(userId, partialName) ?? [];

        return _mapper.Map<IEnumerable<FolderSearchResponseModel>>(folders);
    }

    public async Task<IEnumerable<DeletedFolderModel>> GetAllSoftDeletedAsync(Guid userId)
    {
        var folders = await _repository.Folder.GetAllSoftDeletedAsync(userId, default) ?? [];

        return _mapper.Map<IEnumerable<DeletedFolderModel>>(folders);
    }

    public async Task<DeletedFolderModel> GetSoftDeletedAsync(Guid id)
    {
        var deletedFolder = await _repository.Folder.GetByIdAsync(id);

        return deletedFolder is null
            ? throw new FolderNotFoundException(id)
            : _mapper.Map<DeletedFolderModel>(deletedFolder);
    }
}
