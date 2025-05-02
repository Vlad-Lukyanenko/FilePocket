using FilePocket.BlazorClient.Features.Folders.Models;
using FilePocket.BlazorClient.Helpers;
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
    [Inject] private AppState AppState { get; set; } = default!;

    [Parameter] public string PocketIdParam { get; set; } = string.Empty;
    [Parameter] public string FolderIdParam { get; set; } = string.Empty;
    [Parameter] public int FolderType { get; set; }

    private string _folderName = string.Empty;
    private bool _isDuplicate = false;
    private bool _validName = true;

    private async Task CreateFolderAsync()
    {
        Guid? pocketId = null;

        if (!string.IsNullOrWhiteSpace(PocketIdParam) && PocketIdParam != Guid.Empty.ToString())
        {
            pocketId = Guid.Parse(PocketIdParam);
        }

        Guid? folderId = null;

        if (!string.IsNullOrWhiteSpace(FolderIdParam) && FolderIdParam != Guid.Empty.ToString())
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

        var entitiesName = GetEntitiesName(folder.FolderType);

        AppState.NotifyStateChanged();
        Navigation.NavigateTo(GetGoBackUrl(entitiesName));
    }

    private static string GetEntitiesName(FolderType folderType)
    {
        return folderType switch
        {
            Shared.Enums.FolderType.Notes => "notes",
            Shared.Enums.FolderType.Bookmarks => "bookmarks",
            _ => "files",
        };
    }

    private string GetGoBackUrl(string entitiesName)
    {
        if (string.IsNullOrWhiteSpace(PocketIdParam) && string.IsNullOrWhiteSpace(FolderIdParam))
        {
            return $"/{entitiesName}";
        }

        var folderId = !string.IsNullOrWhiteSpace(FolderIdParam)
            ? Guid.Parse(FolderIdParam)
            : (Guid?)null;


        if (string.IsNullOrWhiteSpace(PocketIdParam) && folderId != null)
        {
            return $"/folders/{folderId}/{entitiesName}";
        }

        if (!string.IsNullOrWhiteSpace(PocketIdParam) && folderId == null)
        {
            return $"/pockets/{PocketIdParam}/{entitiesName}";
        }

        if ((FolderType)FolderType == Shared.Enums.FolderType.Bookmarks)
        {
            return $"/pockets/{PocketIdParam}/bookmarks";
        }        

        return $"/pockets/{PocketIdParam}/folders/{folderId}/{entitiesName}";
    }

    private void NameChanged()
    {
        _validName = !string.IsNullOrEmpty(_folderName);
    }
}
