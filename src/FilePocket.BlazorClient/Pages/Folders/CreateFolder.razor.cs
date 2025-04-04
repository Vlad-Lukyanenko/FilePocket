using FilePocket.BlazorClient.Features.Folders.Models;
using FilePocket.BlazorClient.Services.Folders.Requests;
using FilePocket.BlazorClient.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FilePocket.BlazorClient.Pages.Folders;

public partial class CreateFolder
{
    [Inject] private IFolderRequests FolderRequests { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    [Parameter] public string PocketIdParam { get; set; } = string.Empty;
    [Parameter] public string FolderIdParam { get; set; } = string.Empty;
    [Parameter] public int FolderType { get; set; }

    private string _folderName = string.Empty;
    private bool _isDuplicate = false;
    private bool _validName = true;

    private async Task CreateFolderAsync()
    {
        Guid? pocketId = null;

        if(!string.IsNullOrWhiteSpace(PocketIdParam) && PocketIdParam != Guid.Empty.ToString())
        {
            pocketId = Guid.Parse(PocketIdParam);
        }

        Guid? folderId = null;

        if(!string.IsNullOrWhiteSpace(FolderIdParam) && FolderIdParam != Guid.Empty.ToString())
        {
            folderId = Guid.Parse(FolderIdParam);
        }

        var folder = new FolderModel()
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = _folderName,
            ParentFolderId = folderId,
            PocketId = pocketId,
            FolderType = (FolderType)FolderType
        };

        var result = await FolderRequests.CreateAsync(folder);

        if (!result)
        {
            _isDuplicate = true;
            StateHasChanged();
            return;
        }

        Navigation.NavigateTo(GetGoBackUrl());
    }

    private string GetGoBackUrl()
    {
        if (string.IsNullOrWhiteSpace(PocketIdParam) && string.IsNullOrWhiteSpace(FolderIdParam))
        {
            if((FolderType)FolderType == Shared.Enums.FolderType.Files)
            {
                return "/files";
            }

            if ((FolderType)FolderType == Shared.Enums.FolderType.Bookmarks)
            {
                return "/bookmarks";
            }
        }

        Guid? folderId = null;
        if (!string.IsNullOrWhiteSpace(FolderIdParam))
        {
            folderId = Guid.Parse(FolderIdParam);
        }

        if (string.IsNullOrWhiteSpace(PocketIdParam) && !string.IsNullOrWhiteSpace(FolderIdParam))
        {
            if ((FolderType)FolderType == Shared.Enums.FolderType.Files)
            {
                return $"/folders/{folderId}/files";
            }

            if ((FolderType)FolderType == Shared.Enums.FolderType.Bookmarks)
            {
                return $"/folders/{folderId}/bookmarks";
            }            
        }

        if (!string.IsNullOrWhiteSpace(PocketIdParam) && string.IsNullOrWhiteSpace(FolderIdParam))
        {
            if ((FolderType)FolderType == Shared.Enums.FolderType.Files)
            {
                return $"/pockets/{PocketIdParam}/files";
            }

            if ((FolderType)FolderType == Shared.Enums.FolderType.Bookmarks)
            {
                return $"/pockets/{PocketIdParam}/bookmarks";
            }
        }

        if ((FolderType)FolderType == Shared.Enums.FolderType.Bookmarks)
        {
            return $"/pockets/{PocketIdParam}/folders/{folderId}/bookmarks";
        }

        return $"/pockets/{PocketIdParam}/folders/{folderId}/files";
    }

    private void NameChanged()
    {
        _validName = !string.IsNullOrEmpty(_folderName);
    }
}
