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
    private readonly IFileService _fileService;

    public FolderService(IRepositoryManager repository, IFileService fileService, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
        _fileService = fileService;
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

    public async Task DeleteAsync(Guid folderToDeleteId)
    {
        var stack = new Stack<Guid>();

        stack.Push(folderToDeleteId);

        while (stack.Count > 0)
        {
            var currentFolderId = stack.Pop();
            var currentFolder = await GetFolderAndCheckIfItExistsAsync(currentFolderId);
            var childFolders = _repository.Folder.GetChildFolders(currentFolder.Id).ToList();
            var bookmarks = _repository.Bookmark.GetAllByFolderId(currentFolder.Id).ToList();
            var files = _repository.FileMetadata.GetAllByFoldertId(currentFolder.Id).ToList();

            if (childFolders != null && childFolders.Count != 0)
            {
                foreach (var childFolder in childFolders)
                {
                    if (childFolder.IsDeleted && childFolder.DeletedAt!.Value != currentFolder.DeletedAt!.Value)
                    {
                        childFolder.ParentFolderId = null;
                        _repository.Folder.Update(childFolder);
                    }
                    else
                    {
                        stack.Push(childFolder.Id);
                    }
                }
            }

            if (bookmarks != null && bookmarks.Count != 0)
            {
                foreach (var bookmark in bookmarks)
                {
                    if (bookmark.IsDeleted && bookmark.DeletedAt != currentFolder.DeletedAt)
                    {
                        bookmark.FolderId = null;
                        _repository.Bookmark.UpdateBookmark(bookmark);
                    }
                    else
                    {
                        _repository.Bookmark.DeleteBookmark(bookmark);
                    }
                }
            }

            if (files != null && files.Count != 0)
            {
                foreach (var file in files)
                {
                    if (file.IsDeleted && file.DeletedAt != currentFolder.DeletedAt)
                    {
                        file.FolderId = null;
                        _repository.FileMetadata.UpdateFileMetadata(file);
                    }
                    else
                    {
                        await _fileService.RemoveFileAsync(file.UserId, file.Id);
                    }
                }
            }

            await _repository.Folder.Delete(currentFolder.Id);
        }
    }

    public async Task DeleteByPocketIdAsync(Guid pocketId)
    {
        _repository.Folder.DeleteByPocketId(pocketId);
        await _repository.SaveChangesAsync();
    }

    public async Task MoveToTrashAsync(Guid folderId)
    {
        var folder = await GetFolderAndCheckIfItExistsAsync(folderId);

        await IterateThroughChildFoldersAndMarkAsDeletedOrRestoredAsync(folder, OperationType.Delete);
        await _repository.SaveChangesAsync();
    }

    public async Task RestoreFromTrashAsync(Guid folderId)
    {
        var folder = await GetFolderAndCheckIfItExistsAsync(folderId);

        if (!folder.IsDeleted) return;

        if (folder.ParentFolderId != null)
        {
            try
            {
                await RestoreParentFolderFromTrashAsync(folder.ParentFolderId.Value);
            }
            catch
            {
                folder.ParentFolderId = null;
            }
        }

        await IterateThroughChildFoldersAndMarkAsDeletedOrRestoredAsync(folder, OperationType.Restore);
        await _repository.SaveChangesAsync();
    }

    public async Task RestoreParentFolderFromTrashAsync(Guid folderId)
    {
        var folder = await GetFolderAndCheckIfItExistsAsync(folderId);

        if (!folder.IsDeleted) return;

        folder.RestoreFolderFromDeleted();

        await _repository.SaveChangesAsync();

        if (folder.ParentFolderId != null)
        {
            await RestoreParentFolderFromTrashAsync(folder.ParentFolderId.Value);
        }
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
        var folder = await _repository.Folder.GetByIdAsync(id)
            ?? throw new FolderNotFoundException(id);

        return folder;
    }

    private async Task IterateThroughChildFoldersAndMarkAsDeletedOrRestoredAsync(Folder folder, OperationType operation)
    {
        var deletedAt = operation == OperationType.Delete ? DateTime.UtcNow : folder.DeletedAt!;
        var stack = new Stack<Guid>();

        stack.Push(folder.Id);

        while (stack.Count > 0)
        {
            var currentFolderId = stack.Pop();
            var childFolders = _repository.Folder.GetChildFolders(currentFolderId, true);

            if (childFolders != null && childFolders.Any())
            {
                foreach (var childFolder in childFolders)
                {
                    stack.Push(childFolder.Id);
                }
            }

            var folderToUpdate = await GetFolderAndCheckIfItExistsAsync(currentFolderId);

            if (operation == OperationType.Delete)
            {
                if (folderToUpdate.IsDeleted) continue;

                folderToUpdate.MarkAsDeleted(deletedAt);
            }
            else
            {
                if (folderToUpdate.DeletedAt!.Value != deletedAt.Value) continue;

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
        var directlyDeletedFolders = new List<DeletedFolderModel>();

        foreach (var folder in folders)
        {
            if (folder.ParentFolderId == null)
            {
                directlyDeletedFolders.Add(_mapper.Map<DeletedFolderModel>(folder));
                continue;
            }

            var parentFolder = await GetFolderAndCheckIfItExistsAsync(folder.ParentFolderId.Value);

            if (!parentFolder.IsDeleted || folder.DeletedAt!.Value != parentFolder.DeletedAt!.Value)
            {
                directlyDeletedFolders.Add(_mapper.Map<DeletedFolderModel>(folder));
            }
        }

        return directlyDeletedFolders;
    }

    public async Task<DeletedFolderModel> GetSoftDeletedAsync(Guid id)
    {
        var deletedFolder = await GetFolderAndCheckIfItExistsAsync(id);

        return  _mapper.Map<DeletedFolderModel>(deletedFolder);
    }

    private enum OperationType
    {
        Delete,
        Restore
    }
}
