using FilePocket.Application.Exceptions;
using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Bookmark;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using MapsterMapper;

namespace FilePocket.Application.Services;

public class BookmarkService : IBookmarkService
{
    private readonly IRepositoryManager _repository;
    private readonly IFolderService _folderService;
    private readonly IMapper _mapper;

    public BookmarkService(IRepositoryManager repository, IFolderService folderService,  IMapper mapper)
    {
        _repository = repository;
        _folderService = folderService;
        _mapper = mapper;
    }

    public IEnumerable<BookmarkModel> GetAll(Guid userId, bool isSoftDeleted, bool trackChanges)
    {
        var bookmarks = _repository.Bookmark.GetAll(userId, isSoftDeleted, trackChanges);

        return _mapper.Map<IEnumerable<BookmarkModel>>(bookmarks);
    }

    public async Task<IEnumerable<BookmarkModel>> GetAllAsync(Guid userId, Guid pocketId, Guid? folderId, bool isSoftDeleted, bool trackChanges)
    {
        var bookmarks = await _repository.Bookmark.GetAllAsync(userId, pocketId, folderId, isSoftDeleted, trackChanges);

        return _mapper.Map<List<BookmarkModel>>(bookmarks);
    }

    public async Task<BookmarkModel> CreateBookmarkAsync(BookmarkModel bookmark)
    {
        await AttachBookmarkToPocketAsync(bookmark);
        var bookmarkEntity = _mapper.Map<Bookmark>(bookmark);

        _repository.Bookmark.CreateBookmark(bookmarkEntity);
        await _repository.SaveChangesAsync();

        return _mapper.Map<BookmarkModel>(bookmarkEntity);
    }

    public async Task UpdateBookmarkAsync(UpdateBookmarkRequest bookmark)
    {
        var bookmarkToUpdate = await GetBookmarkAndCheckIfItExistsAsync(bookmark.Id);

        _mapper.Map(bookmark, bookmarkToUpdate);
        await _repository.SaveChangesAsync();        
    }

    public async Task DeleteBookmarkAsync(Guid id)
    {
        var bookmarkToDelete = await GetBookmarkAndCheckIfItExistsAsync(id);
        
        _repository.Bookmark.DeleteBookmark(bookmarkToDelete);
        await _repository.SaveChangesAsync();        
    }

    public async Task MoveToTrashAsync(Guid id)
    {
        var bookmark = await GetBookmarkAndCheckIfItExistsAsync(id);

        bookmark.MarkAsDeleted();

        await _repository.SaveChangesAsync();
    }

    public async Task RestoreFromTrashAsync(Guid id)
    {
        var bookmark = await GetBookmarkAndCheckIfItExistsAsync(id);

        if (bookmark.FolderId != null)
        {
            await _folderService.RestoreParentFolderFromTrashAsync(bookmark.FolderId.Value);
        }

        bookmark.RestoreFromDeleted();

        await _repository.SaveChangesAsync();
    }

    private async Task AttachBookmarkToPocketAsync(BookmarkModel bookmark)
    {
        var pocket = await _repository.Pocket.GetByIdAsync(bookmark.UserId, bookmark.PocketId, trackChanges: true);

        if (pocket is null)
        {
            throw new PocketNotFoundException(bookmark.PocketId);
        }
    }

    private async Task<Bookmark> GetBookmarkAndCheckIfItExistsAsync(Guid id)
    {
        var bookmark = await _repository.Bookmark.GetByIdAsync(id);

        if (bookmark is null)
        {
            throw new BookmarkNotFoundException(id);
        }

        return bookmark;
    }

    public async Task<IEnumerable<BookmarkSearchResponseModel>> SearchAsync(Guid userId, string partialName)
    {
        var bookmarks = await _repository.Bookmark.GetBookmarksByPartialNameAsync(userId, partialName, trackChanges: false);

        return _mapper.Map<List<BookmarkSearchResponseModel>>(bookmarks);
    }

    public async Task<IEnumerable<DeletedBookmarkModel>> GetAllSoftDeletedAsync(Guid userId)
    {
        var bookmarks = await _repository.Bookmark.GetAllSoftdeletedAsync(userId, default) ?? [];
        var directlyDeletedBookmarks = new List<DeletedBookmarkModel>();

        foreach (var bookmark in bookmarks)
        {
            if (bookmark.FolderId == null)
            {
                directlyDeletedBookmarks.Add(_mapper.Map<DeletedBookmarkModel>(bookmark));
                continue;
            }

            var parentFolder = await _repository.Folder.GetByIdAsync(bookmark.FolderId.Value);

            if (!parentFolder!.IsDeleted || bookmark.DeletedAt!.Value != parentFolder.DeletedAt!.Value)
            {
                directlyDeletedBookmarks.Add(_mapper.Map<DeletedBookmarkModel>(bookmark));
            }
        }

        return directlyDeletedBookmarks;
    }

    public async Task<DeletedBookmarkModel> GetSoftDeletedAsync(Guid id)
    {
        var bookmark = await _repository.Bookmark.GetByIdAsync(id);

        return bookmark is null
            ? throw new BookmarkNotFoundException(id)
            : _mapper.Map<DeletedBookmarkModel>(bookmark);
    }

    public async Task DeleteAllBookmarksAsync(Guid userId)
    {
        var bookmarks = _repository.Bookmark.GetAll(userId, true, true);

        _repository.Bookmark.DeleteBookmarks(bookmarks);
        await _repository.SaveChangesAsync();
    }
}
