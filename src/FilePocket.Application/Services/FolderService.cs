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
        await IterateAndMarkAsDeletedThroughChildFoldersAsync(folder.Id);

        folder.MarkAsDeleted();

        await _repository.SaveChangesAsync();
    }

    public async Task<List<FolderModel>> GetAllAsync(Guid userId, Guid? pocketId, Guid? parentFolderId, FolderType folderType, bool isSoftDeleted)
    {
        var result = await _repository.Folder.GetAllAsync(userId, pocketId, parentFolderId, folderType, isSoftDeleted);

        return _mapper.Map<List<FolderModel>>(result);
    }

    private async Task<Folder> GetFolderAndCheckIfItExistsAsync(Guid id)
    {
        var folder = await _repository.Folder.GetAsync(id);

        if (folder is null)
        {
            throw new FolderNotFoundException(id);
        }

        return folder;
    }

    private async Task IterateAndMarkAsDeletedThroughChildFoldersAsync(Guid folderId)
    {
        var stack = new Stack<Guid>();
        stack.Push(folderId);

        while (stack.Count > 0)
        {
            var currentFolderId = stack.Pop();
            var childFolders = _repository.Folder.GetChildFolders(currentFolderId, true);

            if (childFolders is not null && childFolders.Any())
            {
                foreach (var childFolder in childFolders)
                {
                    stack.Push(childFolder.Id);
                }
            }

            var folderToDelete = await GetFolderAndCheckIfItExistsAsync(currentFolderId);
            folderToDelete.IsDeleted = true;
        }
    }
}
