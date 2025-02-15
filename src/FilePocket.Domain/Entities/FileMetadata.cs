﻿using FilePocket.Domain.Models;

namespace FilePocket.Domain.Entities;

public class FileMetadata
{
    public FileMetadata() {}
    private FileMetadata(
        Guid id, Guid userId,
        string originalName, string actualName, 
        string path, FileTypes fileType, double fileSize,
        Guid? pocketId, Guid? folderId, 
        DateTime dateCreated,
        bool isDeleted)
    {
        Id = id;
        OriginalName = originalName;
        ActualName = actualName;
        Path = path;
        FileType = fileType;
        FileSize = fileSize;
        UserId = userId;
        PocketId = pocketId;
        FolderId = folderId;
        DateCreated = dateCreated;
        IsDeleted = isDeleted;
    }

    public Guid Id { get; init; }
    public string OriginalName { get; init; }
    public string ActualName { get; init; }
    public string Path { get; init; }
    public FileTypes FileType { get; init; }
    public double FileSize { get; init; }
    public Guid UserId { get; init; }
    public Guid? PocketId { get; init; }
    public Guid? FolderId { get; init; }
    public DateTime DateCreated { get; init; }
    public bool IsDeleted { get; private set; }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
    }

    public static FileMetadata Create(
        Guid userId, 
        string originalFileName, string filePath, FileTypes fileType, double fileSizeInMbs,
        Guid? pocketId, Guid? folderId)
    {
        var fileId = Guid.NewGuid();
        var actualName = Guid.NewGuid().ToString();

        return new FileMetadata(
            fileId, userId,
            originalFileName, actualName,
            filePath, fileType, fileSizeInMbs,
            pocketId, folderId,
            DateTime.UtcNow,
            isDeleted: false);
    }
}
