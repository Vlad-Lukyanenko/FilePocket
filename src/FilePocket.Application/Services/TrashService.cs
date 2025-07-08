using FilePocket.Application.Interfaces.Services;

namespace FilePocket.Application.Services;

public class TrashService : ITrashService
{
    private readonly IBookmarkService _bookmarkService;
    private readonly IFolderService _folderService;
    private readonly IFileService _fileService;

    public TrashService(IBookmarkService bookmarkService,
        IFolderService folderService,
        IFileService fileService)
    {
        _bookmarkService = bookmarkService;
        _folderService = folderService;
        _fileService = fileService;
    }

    public async Task ClearAllTrashAsync(Guid userId)
    {
        await _bookmarkService.DeleteAllBookmarksAsync(userId);
        await _folderService.DeleteAllFoldersAsync(userId);
        await _fileService.RemoveAllFilesAsync(userId);
    }
}
