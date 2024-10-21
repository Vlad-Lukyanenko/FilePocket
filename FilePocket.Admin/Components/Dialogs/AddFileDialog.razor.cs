using FilePocket.Admin.Models.Storage;
using FilePocket.Admin.Requests;
using FilePocket.Admin.Requests.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Radzen;

namespace FilePocket.Admin.Components.Dialogs
{
    public partial class AddFileDialog : ComponentBase
    {
        [Parameter] public List<StorageModel> Storages { get; set; } = default!;
        [Parameter] public Func<InputFileChangeEventArgs, Guid, string?, Task> OnChange { get; set; } = default!;
        [Parameter] public Func<string, Guid, Task<bool>> BeforeOnChange { get; set; } = default!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] public NotificationService NotificationService { get; set; } = default!;
        [Inject] private IStorageRequests StorageRequests { get; set; } = default!;

        private bool _storageIsSelected = true;
        private bool _fileNameIsCorrect = true;
        private string _fileName = string.Empty;
        private string _fileNameIssueMessage = string.Empty;
        private InputFileChangeEventArgs? _file;
        private StorageModel? _selectedStorage;
        private bool _inProgress;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("addListener");
            await base.OnAfterRenderAsync(firstRender);
        }

        private void HandleReset()
        {
            _storageIsSelected = true;
            _fileNameIsCorrect = true;
            _selectedStorage = null;
            _file = null;
            _fileName = string.Empty;
        }

        private void HandleStorageChange()
        {
            _fileNameIsCorrect = true;
            _storageIsSelected = _selectedStorage != null;
            _file = null;
            _fileName = string.Empty;
        }
        private void HandleInputFileChange(InputFileChangeEventArgs e)
        {
            _storageIsSelected = true;
            _fileNameIsCorrect = true;

            if (_selectedStorage == null)
            {
                _storageIsSelected = false;
                return;
            }

            _file = e;
            _fileName = _file.File.Name;
        }

        private async Task HandleUpload()
        {
            if (_selectedStorage == null || _file == null) return;

            var file = _file.File;
            if (file == null) return;

            if (!ValidateFileName())
            {
                _fileNameIsCorrect = false;
                _fileNameIssueMessage = "File name has invalid characters!";
                return;
            }

            if (await BeforeOnChange.Invoke(_fileName, _selectedStorage.Id))
            {
                _fileNameIsCorrect = false;
                _fileNameIssueMessage = "File with same name already exists!";
                return;
            }

            var newFileSize = file.Size;
            var canUpload = await StorageRequests.CheckStorageCapacity(_selectedStorage.Id, newFileSize);
            if (!canUpload)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Upload Error",
                    Detail = "Storage limit exceeded.",
                    Duration = 4000
                });

                return;
            }

            _inProgress = true;
            StateHasChanged();

            await OnChange.Invoke(_file, _selectedStorage.Id, _fileName);
        }


        private bool ValidateFileName()
        {
            if (string.IsNullOrWhiteSpace(_fileName))
                return false;
            else
                return (_fileName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1);
        }

        private void HandleFileNameInput()
        {
            _fileNameIsCorrect = true;
        }
    }
}
