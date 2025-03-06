﻿using FilePocket.Domain.Entities;

namespace FilePocket.Application.Interfaces.Repositories;

public interface IBookmarkRepository
{
    Task<Bookmark> GetByIdAsync(Guid id);
    IEnumerable<Bookmark> GetAll(Guid userId, bool trackChanges);
    Task<List<Bookmark>> GetAllAsync(Guid userId, Guid pocketId, Guid? folderId, bool trackChanges);
    void CreateBookmark(Bookmark bookmark);
    void DeleteBookmark(Bookmark bookmark);
}
