using FilePocket.Admin.Models.Files;
using FilePocket.Admin.Models.Storage;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace FilePocket.Admin.Components.Dialogs
{
    public partial class DeleteFileDialog : ComponentBase
    {
        [Parameter] public FileModel File { get; set; } = default!;

        [Parameter] public Func<FileModel, Task> OnDeleteButtonClick { get; set; } = default!;

        [Parameter] public DialogService DialogService { get; set; } = default!;

        private async Task HandleDelete() => await OnDeleteButtonClick.Invoke(File);

        private void HandleCancel() => DialogService.Close();
    }
}
