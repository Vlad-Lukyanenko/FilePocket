using FilePocket.BlazorClient.Features.Bookmarks.Requests;
using FilePocket.BlazorClient.Features.Search.Enums;
using FilePocket.BlazorClient.Features.Search.Models;
using FilePocket.BlazorClient.Features.Storage.Models;
using FilePocket.BlazorClient.Features.Storage.Requests;
using FilePocket.BlazorClient.Features.Trash.Models;
using FilePocket.BlazorClient.Features.Trash.Requests;
using FilePocket.BlazorClient.Helpers;
using FilePocket.BlazorClient.Services.Files.Requests;
using FilePocket.BlazorClient.Services.Folders.Requests;
using Microsoft.AspNetCore.Components;
using System.Runtime.InteropServices;

namespace FilePocket.BlazorClient.Pages.DeletedItems
{
    public partial class DeletedItem
    {
        [Parameter]
        public string Id { get; set; } = string.Empty;

        [Parameter]
        public string ItemType { get; set; } = string.Empty;

        [Inject]
        ITrashRequests TrashRequests { get; set; } = default!;

        [Inject]
        IFileRequests FileRequests { get; set; } = default!;

        [Inject]
        IFolderRequests FolderRequests { get; set; } = default!;

        [Inject]
        IBookmarkRequests BookmarkRequests { get; set; } = default!;

        [Inject]
        private IStorageRequests StorageRequests { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private StateContainer<StorageConsumptionModel> StorageStateContainer { get; set; } = default!;

        private SearchResponseModel? _deletedItem;
        private bool _removalProcessStarted;

        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(ItemType))
            {
                return;
            }

            _deletedItem = ItemType.ToLower() switch
            {
                "file" => await TrashRequests.GetSoftDeletedItem<DeletedFileModel>(ItemType, Id),
                "folder" => await TrashRequests.GetSoftDeletedItem<DeletedFolderModel>(ItemType, Id),
                "bookmark" => await TrashRequests.GetSoftDeletedItem<DeletedBookmarkModel>(ItemType, Id),
                _ => throw new NotSupportedException($"Item type '{ItemType}' is not supported."),
            };
        }

        private async void ConfirmDeleteClick()
        {
            switch (ItemType.ToLower())
            {
                case "file":
                    await FileRequests.DeleteFile(Guid.Parse(Id));
                    break;
                case "folder":
                    await FolderRequests.DeleteAsync(Guid.Parse(Id));
                    break;
                case "bookmark":
                    await BookmarkRequests.DeleteAsync(Guid.Parse(Id));
                    break;
                default:
                    throw new NotSupportedException($"Item type '{ItemType}' is not supported.");
            }

            _removalProcessStarted = false;

            Navigation.NavigateTo("/trash");
        }

        private async Task RestoreClick()
        {
            await TrashRequests.RestoreFromTrash(ItemType, Id);

            var storageConsumption = await StorageRequests.GetStorageConsumption();
            StorageStateContainer.SetValue(storageConsumption!);

            Navigation.NavigateTo("/trash");
        }
    }
}
