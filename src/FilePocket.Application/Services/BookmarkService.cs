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
    private readonly IMapper _mapper;

    public BookmarkService(IRepositoryManager repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public IEnumerable<BookmarkModel> GetAll(Guid userId, bool trackChanges)
    {
        var bookmarks = _repository.Bookmark.GetAll(userId, trackChanges);

        return _mapper.Map<IEnumerable<BookmarkModel>>(bookmarks);
    }

    public async Task<IEnumerable<BookmarkModel>> GetAllAsync(Guid userId, Guid pocketId, Guid? folderId, bool trackChanges)
    {
        var bookmarks = await _repository.Bookmark.GetAllAsync(userId, pocketId, folderId, trackChanges);

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
        var bookmarkToUpdate = await _repository.Bookmark.GetByIdAsync(bookmark.Id);

        if (bookmarkToUpdate is not null)
        {
            _mapper.Map(bookmark, bookmarkToUpdate);

            await _repository.SaveChangesAsync();
        }
    }

    public async Task DeleteBookmarkAsync(Guid id)
    {
        var bookmarkToDelete = await _repository.Bookmark.GetByIdAsync(id);

        if (bookmarkToDelete is not null)
        {
            _repository.Bookmark.DeleteBookmark(bookmarkToDelete);
            await _repository.SaveChangesAsync();
        }
    }

    private async Task AttachBookmarkToPocketAsync(BookmarkModel bookmark)
    {
        var pocket = await _repository.Pocket.GetByIdAsync(bookmark.UserId, bookmark.PocketId, trackChanges: true);

        if (pocket is null)
        {
            throw new PocketNotFoundException(bookmark.PocketId);
        }
    }
}
