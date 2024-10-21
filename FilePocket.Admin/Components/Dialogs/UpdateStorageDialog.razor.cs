using FilePocket.Admin.Models.Storage;
using Microsoft.AspNetCore.Components;

namespace FilePocket.Admin.Components.Dialogs
{
    public partial class UpdateStorageDialog :ComponentBase
    {
        [Parameter] public StorageModel Storage { get; set; } = default!;

        [Parameter] public List<StorageModel> Storages { get; set; } = default!;

        [Parameter] public Func<StorageModel, Task> OnSubmit { get; set; } = default!;

        private async Task InvokeOnSubmit() => await OnSubmit.Invoke(Storage);

        private bool ValidateName() => !Storages.Exists(d => d.Id != Storage.Id && d.Name == Storage.Name);
    }
}
