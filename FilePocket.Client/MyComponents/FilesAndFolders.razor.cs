using FilePocket.Client.Services.Files.Models;
using FilePocket.Client.Services.Folders.Requests;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using FilePocket.Client.Features.Folders.Models;
using FilePocket.Client.Services.Files.Requests;
using FilePocket.Client.Pages.Pockets;

namespace FilePocket.Client.MyComponents
{
    public partial class FilesAndFolders
    {
        public const string ClientId = "f666b084-d2a3-4321-b667-31ba3fb7a8fd";

        [Parameter]
        public Guid PocketId { get; set; }

        [Parameter]
        public Guid FolderId { get; set; } = Guid.Empty;

        [Inject]
        private IFileRequests FileRequests { get; set; } = default!;

        [Inject]
        private IFolderRequests FolderRequests { get; set; } = default!;

        //private Guid _pocketId => Guid.Parse(PocketId);

        private ConcurrentBag<FileInfoModel> _cFiles = new();
        private ConcurrentBag<FileInfoModel> _tempFiles = new();
        private List<Guid> _selectedFiles = new();
        private ConcurrentBag<FolderModel> _folders = new();

        private int maxAllowedFiles = 15;
        private bool showInputFile = true;

        private bool _isMasterChecked = false;
        private bool _rmBtnDisabled = true;

        private bool _selectedAll;

        protected override async Task OnInitializedAsync()
        {
            await InitPage();
        }

        private async void GoToFolder(Guid pocketId, Guid? folderId)
        {
            var url = $"/pockets/{pocketId}/folders/{folderId}";

            PocketId = pocketId;

            if(folderId is not null)
            {
                FolderId = folderId.Value; 
            }

            Navigation.NavigateTo(url);
            await InitPage();
            StateHasChanged();
        }

        private async Task InitPage()
        {
            _folders = new ConcurrentBag<FolderModel>();
            _cFiles = new ConcurrentBag<FileInfoModel>();

            List<FileInfoModel> files;
            List<FolderModel> folders;

            if (FolderId == Guid.Empty)
            {
                folders = (await FolderRequests.GetAllAsync(PocketId)).ToList();
                files = await FileRequests.GetFilesAsync(PocketId);
            }
            else
            {   
                folders = (await FolderRequests.GetAllAsync(PocketId, FolderId)).ToList();
                files = await FileRequests.GetFilesAsync(PocketId, FolderId);
            }

            foreach (var file in files)
            {
                _cFiles.Add(file);
            }

            foreach (var folder in folders)
            {
                _folders.Add(folder);
            }
        }

        private void MasterCheckboxChanged()
        {
            _selectedAll = !_selectedAll;

            _rmBtnDisabled = !_selectedAll;

            foreach (var folder in _folders)
            {
                folder.IsSelected = _selectedAll;
            }

            foreach (var file in _cFiles)
            {
                file.IsSelected = _selectedAll;
            }
        }

        private void ChildFileCheckboxChanged(Guid fileId)
        {
            var f = _cFiles.First(c => c.Id == fileId);

            f.IsSelected = !f.IsSelected;

            var selectedFiles = _cFiles.Any(c => c.IsSelected);
            var selectedFolders = _folders.Any(c => c.IsSelected);

            _rmBtnDisabled = !(selectedFiles || selectedFolders);
        }

        private void ChildFolderCheckboxChanged(Guid folderId)
        {
            var f = _folders.First(c => c.Id == folderId);

            f.IsSelected = !f.IsSelected;

            var selectedFiles = _cFiles.Any(c => c.IsSelected);
            var selectedFolders = _folders.Any(c => c.IsSelected);

            _rmBtnDisabled = !(selectedFiles || selectedFolders);
        }

        private async void DeleteSelectedFiles()
        {
            var selectedFiles = _cFiles.Where(c => c.IsSelected);
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

                var tmp = new ConcurrentBag<FolderModel>();

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
                    await FileRequests.DeleteFile(PocketId, file.Id);
                });

                var tmp = new ConcurrentBag<FileInfoModel>();

                foreach (var f in _cFiles)
                {
                    if (!f.IsSelected)
                    {
                        tmp.Add(f);
                    }
                }

                _cFiles = tmp;
            }

            _rmBtnDisabled = true;
            StateHasChanged();
        }

        private async void LoadFiles(InputFileChangeEventArgs e)
        {
            long maxFileSize = 1024 * 1024 * 1024;

            await Parallel.ForEachAsync(e.GetMultipleFiles(maxAllowedFiles), async (file, cancellation) =>
            {
                using (var fileContent = new StreamContent(file.OpenReadStream(maxFileSize)))
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(GetContentType(file.ContentType));

                        var folderId = FolderId == Guid.Empty ? "" : FolderId.ToString();

                        content.Add(content: fileContent, name: "file", fileName: file.Name);
                        content.Add(content: new StringContent(ClientId), "ClientId");
                        content.Add(content: new StringContent(PocketId.ToString()), "PocketId");
                        content.Add(content: new StringContent(folderId), "FolderId");

                        var uploadedFile = await FileRequests.UploadFileAsync(content, PocketId);

                        if (uploadedFile != null)
                        {
                            var fileInfoModel =
                                new FileInfoModel
                                {
                                    Id = uploadedFile.Id,
                                    DateCreated = uploadedFile.DateCreated,
                                    FileSize = uploadedFile.FileSize,
                                    FileType = uploadedFile.FileType,
                                    OriginalName = uploadedFile.OriginalName,
                                    StorageId = uploadedFile.StorageId,
                                    FolderId = FolderId == Guid.Empty ? null : FolderId
                                };

                            _cFiles.Add(fileInfoModel);
                            StateHasChanged();
                        }
                    }
                }
            });

            await ResetFileInput();
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
}
