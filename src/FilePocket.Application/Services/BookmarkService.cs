using AutoMapper;
using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;

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

    public async Task<BookmarkModel> CreateBookmarkAsync(BookmarkModel bookmark)
    {
        var bookmarkEntity = _mapper.Map<Bookmark>(bookmark);

        _repository.Bookmark.CreateBookmark(bookmarkEntity);
        await _repository.SaveChangesAsync();

        return _mapper.Map<BookmarkModel>(bookmarkEntity);
    }
}
