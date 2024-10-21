using FilePocket.Admin.Models.Storage;
using Microsoft.AspNetCore.Components;


namespace FilePocket.Admin.Components.Dialogs
{
    public partial class CreateStorageDialog : ComponentBase
    {
        [Parameter] public AddStorageModel NewStorage { get; set; } = default!;

        [Parameter] public List<StorageModel> Storages { get; set; } = default!;

        [Parameter] public Func<AddStorageModel, Task> OnSubmit { get; set; } = default!;

        private async Task InvokeOnSubmit() => await OnSubmit.Invoke(NewStorage);

        private bool ValidateName() => !Storages.Exists(d => d.Name == NewStorage.Name);
    }
}
