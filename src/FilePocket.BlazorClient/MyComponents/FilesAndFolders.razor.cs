using FilePocket.BlazorClient.Services.Files.Models;
using FilePocket.BlazorClient.Services.Folders.Requests;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using FilePocket.BlazorClient.Features.Folders.Models;
using FilePocket.BlazorClient.Services.Files.Requests;
using FilePocket.BlazorClient.Services.Pockets.Requests;
using System.Collections.ObjectModel;
using FilePocket.BlazorClient.Shared.Models;
using FilePocket.BlazorClient.Shared.Enums;
using FilePocket.BlazorClient.Features.Trash;
using FilePocket.BlazorClient.Features.Users.Requests;
using Microsoft.AspNetCore.Components.Authorization;
using FilePocket.BlazorClient.Features.Storage.Requests;
using FilePocket.BlazorClient.Features.Storage.Models;
using FilePocket.BlazorClient.Helpers;

namespace FilePocket.BlazorClient.MyComponents;

public partial class FilesAndFolders
{
    private string _goBackUrl = string.Empty;

    private ObservableCollection<FileInfoModel> _files = new();
    private ObservableCollection<FileUploadError> _fileUploadErrors = [];
    private ObservableCollection<FolderModel> _folders = new();

    private int maxAllowedFiles = 15;
    private bool _rmBtnDisabled = true;

    private bool _selectedAll;
    private bool _removalProcessStarted;

    private FolderModel? _currentFolder;

    private static SemaphoreSlim semaphore = new SemaphoreSlim(10);

    private bool _firstRender = true;
    private Guid? _iconId;

    [Parameter] public Guid? PocketId { get; set; }
    [Parameter] public Guid? FolderId { get; set; } = null;

    [Inject] private IFileRequests FileRequests { get; set; } = default!;
    [Inject] private IFolderRequests FolderRequests { get; set; } = default!;
    [Inject] private IPocketRequests PocketRequests { get; set; } = default!;
    [Inject] AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] IUserRequests UserRequests { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private IStorageRequests StorageRequests { get; set; } = default!;
    [Inject] private StateContainer<StorageConsumptionModel> StorageStateContainer { get; set; } = default!;
    [Inject] private AppState AppState { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var userName = authState.User.Identity?.Name!;
        var user = await UserRequests.GetByUserNameAsync(userName)!;
        _iconId = user.Profile!.IconId;

        if (PocketId is null)
        {
            var defaultPocket = await PocketRequests.GetDefaultAsync();

            PocketId = defaultPocket.Id;
        }

        await InitPage();
        StateHasChanged();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!_firstRender)
        {
            await OnInitializedAsync();
        }
        else
        {
            _firstRender = false;
        }
    }

    private async Task InitPage()
    {
        List<FileInfoModel> files;
        List<FolderModel> folders;
        var folderTypes = new List<FolderType> { FolderType.Files, FolderType.Documents };

        _currentFolder = FolderId is null ? null : await FolderRequests.GetAsync(PocketId!.Value, FolderId.Value);

        if (FolderId == null)
        {
            folders = (await FolderRequests.GetAllAsync(PocketId, folderTypes, isSoftDeleted: false)).ToList();
            files = await FileRequests.GetFilesAsync(PocketId, null, false);
        }
        else
        {
            folders = (await FolderRequests.GetAllAsync(PocketId, FolderId.Value, folderTypes, isSoftDeleted: false)).ToList();
            files = await FileRequests.GetFilesAsync(PocketId, FolderId.Value, false);
        }


        if (_iconId is not null && _iconId != Guid.Empty)
        {
            _files = new ObservableCollection<FileInfoModel>(files.Where(f => f.Id != _iconId));
        }
        else
        {
            _files = new ObservableCollection<FileInfoModel>(files);
        }

        _folders = new ObservableCollection<FolderModel>(folders);
        _fileUploadErrors = [];

        StateHasChanged();

        _goBackUrl = GetGoBackUrl();
    }

    private async Task DeleteFolderClick()
    {
        if (FolderId is not null)
        {
            await FolderRequests.SoftDeleteAsync(FolderId.Value);
        }

        _removalProcessStarted = false;

        var storageConsumption = await StorageRequests.GetStorageConsumption();
        StorageStateContainer.SetValue(storageConsumption!);
        AppState.NotifyStateChanged();
        Navigation.NavigateTo(_goBackUrl);
    }

    private string GetGoBackUrl()
    {
        var pocketUrl = $"/pockets/{PocketId}";
        var parentFolderUrl = _currentFolder?.ParentFolderId is null ? "" : $"/folders/{_currentFolder!.ParentFolderId}";

        return $"{pocketUrl}{parentFolderUrl}/files";
    }

    private async void UploadFiles(InputFileChangeEventArgs e)
    {
        var files = e.GetMultipleFiles(maxAllowedFiles);
        if (!files.Any())
        {
            Console.WriteLine("No files to process.");
            return;
        }

        var maxFileSize = 1024 * 1024 * 1024;

        var uploadTasks = files.Select(async file =>
        {
            await semaphore.WaitAsync();

            var fileInfoModel = new FileInfoModel
            {
                OriginalName = "Uploading...",
                FolderId = FolderId == Guid.Empty ? null : FolderId,
                IsLoaded = false
            };

            try
            {
                using (var fileContent = new StreamContent(file.OpenReadStream(maxFileSize)))
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(GetContentType(file.ContentType));

                        await InvokeAsync(() =>
                        {
                            _files.Add(fileInfoModel);
                            StateHasChanged();
                        });

                        var folderId = string.Empty;
                        if (FolderId is not null && FolderId != Guid.Empty)
                        {
                            folderId = FolderId.ToString();
                        }

                        var pocketId = PocketId.ToString();

                        content.Add(content: fileContent, name: "file", fileName: file.Name);
                        content.Add(content: new StringContent(pocketId), "PocketId");
                        content.Add(content: new StringContent(folderId), "FolderId");

                        var uploadedFile = await FileRequests.UploadFileAsync(content);
                        if (uploadedFile is not null)
                        {
                            await InvokeAsync(() =>
                            {
                                fileInfoModel.Id = uploadedFile.Id;
                                fileInfoModel.CreatedAt = uploadedFile.CreatedAt;
                                fileInfoModel.FileSize = uploadedFile.FileSize;
                                fileInfoModel.FileType = uploadedFile.FileType;
                                fileInfoModel.OriginalName = uploadedFile.OriginalName;
                                fileInfoModel.PocketId = uploadedFile.PocketId;
                                fileInfoModel.FolderId = FolderId == Guid.Empty ? null : FolderId;
                                fileInfoModel.IsLoaded = true;

                                StateHasChanged();
                            });

                            var storageConsumption = await StorageRequests.GetStorageConsumption();
                            StorageStateContainer.SetValue(storageConsumption!);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file {file.Name}: {ex.Message}");

                await InvokeAsync(() =>
                {
                    _files.Remove(fileInfoModel);

                    if (ex is RequestHandlingErrorException exception)
                    {
                        var error = exception.Error;
                        _fileUploadErrors.Add(new FileUploadError { OriginalName = file.Name, ErrorMessage = error.Message! });
                    }
                    else
                    {
                        _fileUploadErrors.Add(new FileUploadError { OriginalName = file.Name, ErrorMessage = ex.Message });
                    }

                    StateHasChanged();
                });
            }
            finally
            {
                semaphore.Release();
            }
        }).ToList();

        await Task.WhenAll(uploadTasks);

        await ResetFileInput();
    }

    private void GoBack()
    {
        Navigation.NavigateTo(Navigation.ToBaseRelativePath(Navigation.Uri));
    }

    private string GetFolderUrl(Guid? pocketId, Guid? folderId)
    {
        if (pocketId is null)
        {
            return $"/folders/{folderId}/files";
        }

        return $"/pockets/{pocketId}/folders/{folderId}/files";
    }

    private void MasterCheckboxChanged()
    {
        _selectedAll = !_selectedAll;

        _rmBtnDisabled = !_selectedAll;

        foreach (var folder in _folders)
        {
            folder.IsSelected = _selectedAll;
        }

        foreach (var file in _files)
        {
            file.IsSelected = _selectedAll;
        }
    }

    private void ChildFileCheckboxChanged(Guid fileId)
    {
        var f = _files.First(c => c.Id == fileId);

        f.IsSelected = !f.IsSelected;

        var selectedFiles = _files.Any(c => c.IsSelected);
        var selectedFolders = _folders.Any(c => c.IsSelected);

        _rmBtnDisabled = !(selectedFiles || selectedFolders);
    }

    private void ChildFolderCheckboxChanged(Guid folderId)
    {
        var f = _folders.First(c => c.Id == folderId);

        f.IsSelected = !f.IsSelected;

        var selectedFiles = _files.Any(c => c.IsSelected);
        var selectedFolders = _folders.Any(c => c.IsSelected);

        _rmBtnDisabled = !(selectedFiles || selectedFolders);
    }

    private async void DeleteSelectedFiles()
    {
        var selectedFiles = _files.Where(c => c.IsSelected);
        var selectedFolders = _folders.Where(c => c.IsSelected);

        if (!selectedFiles.Any() && !selectedFolders.Any())
        {
            return;
        }

        if (selectedFolders.Any())
        {
            await Parallel.ForEachAsync(selectedFolders, async (folder, cancellation) =>
            {
                await FolderRequests.DeleteAsync(folder.Id!.Value);
            });

            var tmp = new ObservableCollection<FolderModel>();

            foreach (var f in _folders)
            {
                if (!f.IsSelected)
                {
                    tmp.Add(f);
                }
            }

            _folders = tmp;
        }

        if (selectedFiles.Any())
        {
            await Parallel.ForEachAsync(selectedFiles, async (file, cancellation) =>
            {
                await FileRequests.DeleteFile(file.Id);
            });

            var tmp = new ObservableCollection<FileInfoModel>();

            foreach (var f in _files)
            {
                if (!f.IsSelected)
                {
                    tmp.Add(f);
                }
            }

            _files = tmp;
        }

        _rmBtnDisabled = true;
        StateHasChanged();
    }

    private static string GetContentType(string fileContentType)
    {
        return string.IsNullOrWhiteSpace(fileContentType) ? "application/octet-stream" : fileContentType;
    }

    private async Task ResetFileInput()
    {
        await JS.InvokeVoidAsync("resetFileInput", "fileInput");
    }
}
