using FilePocket.Admin.Components.Dialogs;
using FilePocket.Admin.Models;
using FilePocket.Admin.Models.Files;
using FilePocket.Admin.Requests.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Radzen;
using System.Net.Http.Headers;


namespace FilePocket.Admin.Pages
{
    public partial class Files
    {
        private List<Models.Storage.StorageModel>? _storages;
        private LoggedInUserModel? _user;
        private FilesFilterOptionsModel _filterOptions = new();
        private FilteredFilesModel _filteredFiles = new();
        private readonly List<int> _pageSizes = [15, 25, 50];

        [Inject] private IStorageRequests StorageRequests { get; set; } = default!;

        [Inject] private IFileRequests FileRequests { get; set; } = default!;

        [Inject] private IUserRequests UserRequests { get; set; } = default!;

        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject] private DialogService DialogService { get; set; } = default!;

        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var userName = user.Identity?.Name!;
            _user = await UserRequests.GetByUserNameAsync(userName);

            if (_user == null) return;

            _filterOptions.UserId = (Guid)_user!.Id!;
            _filterOptions.PageSize = 15;
            _filterOptions.PageNumber = 1;

            await LoadStorages();
            await LoadFilteredFiles();
        }

        #region Loaders
        private async Task LoadStorages()
        {
            _storages = (await StorageRequests.GetAllAsync()).Where(x => x.UserId == _user!.Id).ToList();
        }

        private async Task LoadFilteredFiles()
        {
            if (_storages == null) await LoadStorages();
            _filteredFiles = await FileRequests.GetFilteredAsync(_filterOptions);
        }
        #endregion

        #region File Operations Handlers
        private async Task ShowAddFileDialog()
        {
            if (_storages == null) return;

            await DialogService
                .OpenAsync<AddFileDialog>("Add File",
                new Dictionary<string, object>()
                {
                    {"Storages", _storages },
                    { "OnChange",  (Func<InputFileChangeEventArgs, Guid, string?, Task>)OnInputFileChange},
                    { "BeforeOnChange", (Func<string, Guid, Task<bool>>)CheckIfFileExists}
                },
                new DialogOptions() { Width = "450px" });
        }

        private async Task<bool> CheckIfFileExists(string fileName, Guid storageId)
        {
            return await FileRequests.CheckIfFileExists(fileName, storageId);
        }

        private async Task OnInputFileChange(InputFileChangeEventArgs e, Guid storageId, string? fileName)
        {
            if (e == null) return;

            var file = e.File;
            long maxFileSize = 1024 * 1024 * 1024;
            var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));

            using var content = new MultipartFormDataContent();

            fileContent.Headers.ContentType = new MediaTypeHeaderValue(GetContentType(file.ContentType));

            content.Add(
                content: fileContent,
                name: "\"file\"",
                fileName: fileName ?? e.File.Name);

            var lastAddedFileInStorage = await FileRequests.PostAsync(content, storageId);

            if (lastAddedFileInStorage != null)
            {
                _filteredFiles.Files!.Add(lastAddedFileInStorage);
            }

            DialogService.Close();

            await LoadFilteredFiles();

            StateHasChanged();
        }

        private async Task OnInputFileChange2(InputFileChangeEventArgs e, Guid storageId, string? fileName)
        {
            if (e == null) return;

            var file = e.File;
            long maxFileSize = 1024 * 1024 * 1024;

            var session = await FileRequests.CreateSession(_user!.Id!.Value, storageId, new Domain.Models.CreateSessionParams()
            {
                FileSize = file.Size,
                FileName = fileName,
            });

            var buffer = new byte[session.ChunkSize];
            var bytesRead = 0;
            var chunkNumber = 0;
            var contentType = new MediaTypeHeaderValue(GetContentType(file.ContentType));

            using (var stream = file.OpenReadStream(maxFileSize))
            {
                while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
                {
                    var chunk = new byte[bytesRead];
                    Array.Copy(buffer, chunk, bytesRead);
                    await UploadChunk(chunk, ++chunkNumber, session, contentType);
                }
            }

            var lastAddedFileInStorage = (await FileRequests.GetAllAsync(storageId)).LastOrDefault();

            if (lastAddedFileInStorage != null)
            {
                _filteredFiles.Files!.Add(lastAddedFileInStorage);
            }

            DialogService.Close();

            await LoadFilteredFiles();

            StateHasChanged();
        }

        private async Task ShowDeleteFileDialog(FileModel file)
        {

            await DialogService
                .OpenAsync<DeleteFileDialog>(
                "Delete File",
                new Dictionary<string, object>()
                {
                    {"File",  file},
                    {"DialogService", DialogService },
                    {"OnDeleteButtonClick", (Func<FileModel, Task>)DeleteFile }
                },
                new DialogOptions() { Width = "450px" });
        }

        private async Task DeleteFile(FileModel file)
        {
            var result = await FileRequests.DeleteAsync(file.Id, file.StorageId);

            DialogService.Close();

            if (result)
            {
                await LoadFilteredFiles();

                StateHasChanged();
            }

        }

        private async Task DownloadFile(FileModel file)
        {
            var response = await FileRequests.GetById(file.Id, file.StorageId);

            if (response == null) return;
            if (response.GetType() != typeof(FileDownloadModel)) return;

            var base64Data = Convert.ToBase64String(response.FileByteArray);

            await JSRuntime.InvokeVoidAsync("saveAsFile", response.OriginalName, base64Data);
        }

        private async Task CopyIdToClipboard(FileModel file)
        {
            await JSRuntime.InvokeVoidAsync("copyToClipboard", file.Id);
        }

        private static bool IsImageFile(string fileType)
        {
            return fileType.ToLower() == "image";
        }


        private async Task ShowImageThumbnaiClick(FileModel file)
        {
            Console.WriteLine($"File clicked: {file.OriginalName}");

            if (IsImageFile(file.FileType!))
            {
                await ShowThumbnailDialog(file);
            }
        }

        private async Task ShowThumbnailDialog(FileModel file)
        {
            Guid storageId = file.StorageId;
            Guid imageId = file.Id;
            int size = 500;

            var imageResponse = await FileRequests.GetThumbnailAsync(storageId, imageId, size);

            if (imageResponse != null)
            {
                string base64Image = Convert.ToBase64String(imageResponse.FileByteArray!);
                var imageDataUrl = $"data:image/jpeg;base64,{base64Image}";

                await DialogService.OpenAsync<ThumbnailDialog>(
                    imageResponse.OriginalName,
                    new Dictionary<string, object>
                    {
                { "ImageUrl", imageDataUrl },
                { "FileName", imageResponse.OriginalName! },
                { "DateCreated", imageResponse.DateCreated },
                { "FileSize", imageResponse.FileSize! }
                    },
                    new DialogOptions
                    {
                        Width = "800px",
                        Height = "600px",
                        Resizable = false,
                        Draggable = false
                    });
            }
            else
            {
                Console.WriteLine("Image is not found.");
            }
        }

        #endregion

        #region Filters
        private async Task SelectedStorageChanged(string name)
        {
            var selectedStorage = _storages!.SingleOrDefault(s => s.Name == name);

            if (selectedStorage != null)
            {
                _filterOptions.StorageId = selectedStorage.Id;
                _filterOptions.StorageName = name;
            }
            else
            {
                _filterOptions.StorageId = null;
                _filterOptions.StorageName = string.Empty;
            }

            await LoadFilteredFiles();
        }

        private async Task FileNameContainsChanged(ChangeEventArgs e)
        {
            if (e.Value == null) { await LoadFilteredFiles(); return; }
            _filterOptions.OriginalNameContains = e.Value.ToString() ?? string.Empty;

            await LoadFilteredFiles();
        }

        private async Task AfterDateChanged(DateTime? date)
        {
            _filterOptions.AfterDate = date;
            if (_filterOptions.BeforeDate.HasValue && date.HasValue && _filterOptions.BeforeDate > date)
                _filterOptions.BeforeDate = date;
            await LoadFilteredFiles();
        }

        private async Task BeforeDateChanged(DateTime? date)
        {
            _filterOptions.BeforeDate = date;
            if (_filterOptions.AfterDate.HasValue && date.HasValue && _filterOptions.AfterDate > date)
                _filterOptions.AfterDate = date;

            await LoadFilteredFiles();
        }

        private async Task FileTypeChanged(string fileType)
        {
            _filterOptions.FileType = fileType == "All Types" ? string.Empty : fileType;

            await LoadFilteredFiles();
        }

        private async Task ResetButtonClick()
        {
            var pageSize = _filterOptions.PageSize;

            _filterOptions = new()
            {
                UserId = (Guid)_user!.Id!,
                PageSize = pageSize,
                PageNumber = 1
            };

            await LoadFilteredFiles();
        }

        #endregion

        #region Pagination
        private async Task GoToFirstPage()
        {
            _filterOptions.PageNumber = 1;
            await LoadFilteredFiles();
        }

        private async Task GoToLastPage()
        {
            _filterOptions.PageNumber = _filteredFiles.PagesCount;
            await LoadFilteredFiles();
        }

        private async Task GoToNextPage()
        {
            if (_filterOptions.PageNumber == _filteredFiles.PagesCount) return;
            ++_filterOptions.PageNumber;
            await LoadFilteredFiles();
        }

        private async Task GoToPrevPage()
        {
            if (_filterOptions.PageNumber == 1) return;
            --_filterOptions.PageNumber;
            await LoadFilteredFiles();
        }

        private async Task OnPageSizeChanged()
        {
            _filterOptions.PageNumber = 1;
            await LoadFilteredFiles();
        }
        #endregion

        private static string GetContentType(string fileContentType)
        {
            return string.IsNullOrWhiteSpace(fileContentType) ? "application/octet-stream" : fileContentType;
        }

        private async Task UploadChunk(byte[] chunk, int chunkNumber, SessionModel session, MediaTypeHeaderValue contentType)
        {
            var chunkContetnt = new ByteArrayContent(chunk);

            using var content = new MultipartFormDataContent();
            chunkContetnt.Headers.ContentType = contentType;
            content.Add(
                content: chunkContetnt,
                name: "\"file\"",
                fileName: session.FileName);

            await FileRequests.PostAsync(content, session, chunkNumber);
        }
    }
}
