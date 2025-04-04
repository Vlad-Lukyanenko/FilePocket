using FilePocket.BlazorClient.Features.Folders.Models;
using FilePocket.BlazorClient.Services.Files.Models;
using FilePocket.BlazorClient.Services.Files.Requests;
using FilePocket.BlazorClient.Services.Folders.Requests;
using FilePocket.BlazorClient.Services.Pockets.Requests;
using FilePocket.BlazorClient.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.ObjectModel;

namespace FilePocket.BlazorClient.Pages.Files;

public partial class DeletedFiles
{
    private ObservableCollection<FolderModel> _folders = new();
    private ObservableCollection<FileInfoModel> _files = new();
    private bool _loading = true;
    private string _pageTitle = string.Empty;
    private FolderModel? _currentFolder;
    private bool _deleteFolderStarted;
    private bool _restoreFolderStarted;
    private Guid _fileIdToBeDeleted;
    private Guid _fileIdToBeRestored;

    [Parameter] public Guid PocketId { get; set; }
    [Parameter] public Guid? FolderId { get; set; }

    [Inject] private IPocketRequests PocketRequests { get; set; } = default!;
    [Inject] private IFolderRequests FolderRequests { get; set; } = default!;
    [Inject] private IFileRequests FileRequests { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        List<FileInfoModel> files;
        List<FolderModel> folders;

        _currentFolder = FolderId is null ? null : await FolderRequests.GetAsync(FolderId.Value);
        var currentFolderName = _currentFolder is null ? string.Empty : $" - {_currentFolder.Name}";
        var currentFolderUrl = FolderId is null ? "" : $"{FolderId}/";

        _pageTitle = $"Deleted files{currentFolderName}";

        if (PocketId == Guid.Empty)
        {
            var defaultPocket = await PocketRequests.GetDefaultAsync();
            PocketId = defaultPocket.Id;
        }

        if (FolderId is null)
        {
            folders = (await FolderRequests.GetAllAsync(PocketId, FolderType.Files, isSoftDeleted: true)).ToList();
            files = await FileRequests.GetFilesAsync(PocketId, null, true);
        }
        else
        {
            folders = (await FolderRequests.GetAllAsync(PocketId, FolderId.Value, FolderType.Files, isSoftDeleted: true)).ToList();
            files = await FileRequests.GetFilesAsync(PocketId, FolderId.Value, true);
        }

        _folders = new ObservableCollection<FolderModel>(folders);
        _files = new ObservableCollection<FileInfoModel>(files);
        _loading = false;
    }

    private void DeleteFileClicked(FileInfoModel file)
    {
        _fileIdToBeDeleted = file.Id;
        _fileIdToBeRestored = default;
    }

    private void RestoreFileClicked(FileInfoModel file)
    {
        _fileIdToBeDeleted = default;
        _fileIdToBeRestored = file.Id;
    }

    private async Task ConfirmFileDeletionAsync()
    {
        var file = _files.FirstOrDefault(f => f.Id == _fileIdToBeDeleted);

        if (file is not null)
        {
            var isDeleted = await FileRequests.DeleteFile(file.Id);

            if (isDeleted)
            {
                _files.Remove(file);
                _fileIdToBeDeleted = default;
            }
        }
    }

    private void CancelClicked()
    {
        _fileIdToBeDeleted = default;
        _fileIdToBeRestored = default;
    }

    private async Task DeleteFolderClickAsync()
    {
        if (FolderId is not null)
        {
            await FolderRequests.DeleteAsync(FolderId.Value);
        }

        _deleteFolderStarted = false;

        await JSRuntime.InvokeVoidAsync("history.back");
    }

    private async Task RestoreFolderClickAsync()
    {
        if (FolderId is not null)
        {
            await FolderRequests.RestoreAsync(FolderId.Value);
        }

        _restoreFolderStarted = false;

        await JSRuntime.InvokeVoidAsync("history.back");
    }
}
