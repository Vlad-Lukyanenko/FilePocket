﻿namespace FilePocket.Application.Interfaces.Services;

public interface IServiceManager
{
    IPocketService PocketService { get; }

    ISharedFileService SharedFileService { get; }

    IFileService FileService { get; }

    IFolderService FolderService { get; }

    IAuthenticationService AuthenticationService { get; }

    IBookmarkService BookmarkService { get; }

    IProfileService ProfileService { get; }
}
