using FilePocket.Client.Services.Files.Models;
using FilePocket.Client.Services.Files.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Collections.Concurrent;
using System.Net.Http.Headers;

namespace FilePocket.Client.Pages.Files
{
    public partial class PocketFiles
    {
        public const string ClientId = "f666b084-d2a3-4321-b667-31ba3fb7a8fd";

        [Parameter]
        public string PocketIdParam { get; set; } = string.Empty;

        [Inject]
        private IFileRequests FileRequests { get; set; } = default!;

        private Guid _pocketId => Guid.Parse(PocketIdParam);

        private ConcurrentBag<FileInfoModel> _cFiles = new();
        private ConcurrentBag<FileInfoModel> _tempFiles = new();
        private List<Guid> _selectedFiles = new();

        private int maxAllowedFiles = 15;
        private bool showInputFile = true;

        private bool _isMasterChecked = false;
        private bool _rmBtnDisabled = true;

        private bool _selectedAll;

        protected override async Task OnInitializedAsync()
        {
            var files = await FileRequests.GetFilesAsync(_pocketId);

            foreach (var file in files)
            {
                _cFiles.Add(file);
            }
        }

        private void MasterCheckboxChanged()
        {
            _selectedAll = !_selectedAll;

            _rmBtnDisabled = !_selectedAll;

            foreach (var file in _cFiles)
            {
                file.IsSelected = _selectedAll;
            }
        }

        private void ChildCheckboxChanged(Guid fileId)
        {
            var f = _cFiles.First(c => c.Id == fileId);

            f.IsSelected = !f.IsSelected;

            _rmBtnDisabled = !_cFiles.Any(c => c.IsSelected);
        }

        private async void DeleteSelectedFiles()
        {
            var selectedFiles = _cFiles.Where(c => c.IsSelected);

            if (!selectedFiles.Any())
            {
                return;
            }

            await Parallel.ForEachAsync(selectedFiles, async (file, cancellation) =>
            {
                await FileRequests.DeleteFile(_pocketId, file.Id);
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

                        content.Add(content: fileContent, name: "file", fileName: file.Name);
                        content.Add(content: new StringContent(ClientId), "ClientId" );
                        content.Add(content: new StringContent(PocketIdParam), "PocketId" );

                        var uploadedFile = await FileRequests.UploadFileAsync(content, _pocketId);

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
                                    StorageId = uploadedFile.StorageId
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
