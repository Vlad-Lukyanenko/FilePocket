﻿using FilePocket.Domain.Entities.Abstractions;
using FilePocket.Domain.Enums;
using FilePocket.Domain.Extensions;

namespace FilePocket.Domain.Entities;

public class Folder : IAmSoftDeletedEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PocketId { get; set; }
    public Guid? ParentFolderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public FolderType FolderType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public virtual ICollection<Bookmark>? Bookmarks { get; set; }
    public virtual ICollection<FileMetadata>? FileMetadata { get; set; }

    public void MarkAsDeleted(DateTime? deletedAt = null)
    {
        IsDeleted = true;
        DeletedAt = deletedAt ?? DateTime.UtcNow;

        if (Bookmarks is not null && Bookmarks.Any())
        {
            Bookmarks?.ForEach(b =>
            {
                if (!b.IsDeleted)
                {
                    b.MarkAsDeleted(DeletedAt);
                }
            });
        }

        if (FileMetadata is not null && FileMetadata.Any())
        {
            FileMetadata?.ForEach(f =>
            {
                if (!f.IsDeleted)
                {
                    f.MarkAsDeleted(DeletedAt);
                }
            });
        }
    }

    public void RestoreFromDeleted()
    {
        var deletedAt = DeletedAt;

        RestoreFolderFromDeleted();

        if (Bookmarks is not null && Bookmarks.Any())
        {
            Bookmarks?.ForEach(b =>
            {
                if (b.DeletedAt == deletedAt)
                {
                    b.RestoreFromDeleted();
                }
            });
        }

        if (FileMetadata is not null && FileMetadata.Any())
        {
            FileMetadata?.ForEach(f =>
            {
                if (f.DeletedAt == deletedAt)
                {
                    f.RestoreFromDeleted();
                }
            });
        }
    }

    public void RestoreFolderFromDeleted()
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}
