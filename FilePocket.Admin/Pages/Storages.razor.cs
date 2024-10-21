using FilePocket.Admin.Components.Dialogs;
using FilePocket.Admin.Models;
using FilePocket.Admin.Models.Storage;
using FilePocket.Admin.Requests.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Radzen;


namespace FilePocket.Admin.Pages;

public partial class Storages
{
    private List<StorageModel> _storages = default!;

    private StorageModel _storage = default!;

    private LoggedInUserModel? _user;

    private string _userName = string.Empty;

    [Inject] private IStorageRequests StorageRequests { get; set; } = default!;

    [Inject] IUserRequests UserRequests { get; set; } = default!;

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    [Inject] private DialogService DialogService { get; set; } = default!;

    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        _userName = user.Identity?.Name!;
        _user = await UserRequests.GetByUserNameAsync(_userName);
        if (_user == null) return;

        await LoadStorages();
    }

    private async Task LoadStorages()
    {
        _storages = (await StorageRequests.GetAllAsync()).Where(x => x.UserId == _user!.Id).ToList();
    }

    private async Task LoadStorageInfo(Guid id)
    {
        _storage = (await StorageRequests.GetDetails(id));
    }

    private async Task ShowStorageDetailsDialog(StorageModel storage)
    {
        if (_user == null || storage == null) return;

        var detailsStorage = await StorageRequests.GetDetails(storage.Id);

        var result = await DialogService
        .OpenAsync<StorageDetailsDialog>("Storage Information",
        new Dictionary<string, object>()
            {
                { "Storage", detailsStorage },
                { "OnSubmit", async (StorageModel model) => await LoadStorageInfo(model.Id) }
            },
        new DialogOptions() { Width = "450px;" });
    }

    private async Task AddStorage(AddStorageModel storage)
    {
        var created = await StorageRequests.PostAsync(storage);

        if (!created) return;

        await LoadStorages();

        DialogService.Close();
    }

    private async Task ShowCreateStorageDialog()
    {
        if (_user == null) return;

        var storage = new AddStorageModel
        {
            UserId = _user.Id!.Value,
        };

        var result = await DialogService
        .OpenAsync<CreateStorageDialog>("Create Storage",
        new Dictionary<string, object>()
            {
                { "NewStorage", storage},
                { "Storages", _storages},
            { "OnSubmit", (Func<AddStorageModel, Task>)AddStorage}
        },
        new DialogOptions() { Width = "450px;" });
    }

    private async Task RenameStorage(StorageModel storage)
    {
        var result = await StorageRequests.PutAsync(storage);

        if (!result) return;

        await LoadStorages();

        DialogService.Close();
    }

    private async Task ShowUpdateStorageDialog(StorageModel storage)
    {
        var result = await DialogService
        .OpenAsync<UpdateStorageDialog>("Rename Storage",
        new Dictionary<string, object>()
            {
            { "Storage", storage},
            { "Storages", _storages},
            { "OnSubmit", (Func<StorageModel, Task>)RenameStorage}
        },
        new DialogOptions() { Width = "450px;" });
    }

    private async Task CopyStorageIdToClipboard(StorageModel storage)
    {
        await JSRuntime.InvokeVoidAsync("copyToClipboard", storage.Id);
    }

    private async Task DeleteStorage(Guid id)
    {
        var result = await StorageRequests.DeleteAsync(id);

        if (!result) return;

        await LoadStorages();
    }
}