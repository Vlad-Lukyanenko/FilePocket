﻿using FilePocket.BlazorClient.Features.Storage.Models;
using FilePocket.BlazorClient.Features.Storage.Requests;
using FilePocket.BlazorClient.Features.Users.Models;
using FilePocket.BlazorClient.Features.Users.Requests;
using FilePocket.BlazorClient.Helpers;
using FilePocket.BlazorClient.Services.Files.Models;
using FilePocket.BlazorClient.Services.Files.Requests;
using FilePocket.BlazorClient.Services.Pockets.Requests;
using FilePocket.BlazorClient.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace FilePocket.BlazorClient.Layout;

public partial class MainLayout : IDisposable
{
    private string? _iconName;
    LoggedInUserModel? _user;
    private string? _icon;
    private string? _parialNameToSearch;
    private StorageConsumptionModel _storageConsumption = new();
    private string _occupiedStorageSpacePercentage = "0";
    public required Dictionary<FileTypes, double> _occupiedSpaceByFileType;
    private string _displayUserName = string.Empty;

    [Inject] AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] IUserRequests UserRequests { get; set; } = default!;
    [Inject] private IFileRequests FileRequests { get; set; } = default!;
    [Inject] private IPocketRequests PocketRequests { get; set; } = default!;
    [Inject] private IStorageRequests StorageRequests { get; set; } = default!;
    [Inject] private StateContainer<LoggedInUserModel> UserStateContainer { get; set; } = default!;
    [Inject] private StateContainer<StorageConsumptionModel> StorageStateContainer { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity is { IsAuthenticated: true })
        {
            var userName = user.Identity?.Name;

            if (!string.IsNullOrEmpty(userName))
            {
                _user = await UserRequests.GetByUserNameAsync(userName);

                if (_user is not null)
                {
                    _displayUserName = GetDisplayedName();
                    StateHasChanged();

                    _ = ShowAvatar();
                    _ = LoadStorageAndDrawChartAsync();
                }
            }
        }

        Navigation.LocationChanged += OnLocationChanged;
        UserStateContainer.OnStateChange += async () => await UpdateUserStateAsync();
        StorageStateContainer.OnStateChange += async () => await UpdateStorageStateAsync();
        NavigationHistory.AddToHistory(Navigation.Uri);
    }

    private async Task ShowAvatar()
    {
        var firstName = string.IsNullOrEmpty(_user!.FirstName) ? string.Empty : _user.FirstName.AsSpan(0, 1);
        var lastName = string.IsNullOrEmpty(_user.LastName) ? string.Empty : _user.LastName.AsSpan(0, 1);
        _iconName = string.Concat(firstName, lastName).ToUpper();

        if (_iconName.Length == 0 && !string.IsNullOrEmpty(_user.UserName))
        {
            _iconName = _user.UserName[..1].ToUpper();
        }

        if (_user.Profile is not null && _user.Profile.IconId is not null && _user.Profile.IconId != Guid.Empty)
        {
            var avatar = await FileRequests.GetImageThumbnailAsync((Guid)_user.Profile.IconId, 500);
            _icon = Convert.ToBase64String(avatar.FileByteArray!);
        }

        StateHasChanged();
    }

    private async Task LoadStorageAndDrawChartAsync()
    {
        try
        {
            _storageConsumption = await StorageRequests.GetStorageConsumption();
            GetStorageConsumptionInPercantage();
            await GetOccupiedSpaceByFileType();
            GetSizeForStorageItems();
            await DrawDoughnutChart();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to Draw Doughnut Chart: {ex.Message}");
        }

        StateHasChanged();
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (SkipAddToHistory(e.Location)) return;
        NavigationHistory.AddToHistory(e.Location);
    }

    private bool SkipAddToHistory(string url)
    {
        return url.Contains("search-results", StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        Navigation.LocationChanged -= OnLocationChanged;
        UserStateContainer.OnStateChange -= async () => await UpdateUserStateAsync();
        StorageStateContainer.OnStateChange -= async () => await UpdateStorageStateAsync();
        GC.SuppressFinalize(this);
    }

    private string IsActive(string href)
    {
        return Navigation.Uri.EndsWith(href, StringComparison.OrdinalIgnoreCase) ? "active" : "";
    }

    private async Task CloseOffcanvasAndNavigate(string url)
    {
        await JS.InvokeVoidAsync("closeOffcanvasMenu");
        Navigation.NavigateTo(url);
    }

    private void NavigateToProfile()
    {
        Navigation.NavigateTo("/profile", forceLoad: false);
    }


    private string GetDisplayedName()
    {
        if (_user is null || (string.IsNullOrEmpty(_user.FirstName) && string.IsNullOrEmpty(_user.LastName)))
        {
            return string.Empty;
        }

        if (!string.IsNullOrEmpty(_user.FirstName) && !string.IsNullOrEmpty(_user.LastName))
        {
            return $"{_user.FirstName} {_user.LastName}";
        }

        return _user.Email!;
    }

    private async Task UpdateUserStateAsync()
    {
        _user = UserStateContainer.Value;

        if (_user!.Profile!.IconId is not null && _user.Profile!.IconId != Guid.Empty)
        {
            var avatar = await FileRequests.GetImageThumbnailAsync((Guid)_user.Profile.IconId, 500);
            _icon = Convert.ToBase64String(avatar.FileByteArray!);
        }

        await InvokeAsync(StateHasChanged);
    }
    private void GetStorageConsumptionInPercantage()
    {
        double proportionOfOccupiedSpace = Math.Round(((_storageConsumption.Used / _storageConsumption.Total) * 100), 2);
        _occupiedStorageSpacePercentage = proportionOfOccupiedSpace.ToString().Replace(',', '.');
    }

    private async Task UpdateStorageStateAsync()
    {
        _storageConsumption = StorageStateContainer.Value!;
        await GetOccupiedSpaceByFileType();
        GetSizeForStorageItems();
        await JS.InvokeVoidAsync("destroyStorageChart");
        await DrawDoughnutChart();
        await InvokeAsync(StateHasChanged);
    }

    private async Task GetOccupiedSpaceByFileType()
    {
        var pocketId = await PocketRequests.GetDefaultAsync();
        List<FileInfoModel> files = await FileRequests.GetAllFilesWithSoftDeletedAsync(pocketId.Id);
        CreateNewFileTypeDictionary();
        foreach (FileInfoModel file in files)
        {
            switch (file.FileType)
            {
                case FileTypes.Document:
                    _occupiedSpaceByFileType[FileTypes.Document] += file.FileSize;
                    break;
                case FileTypes.EBook:
                    _occupiedSpaceByFileType[FileTypes.EBook] += file.FileSize;
                    break;
                case FileTypes.Audio:
                    _occupiedSpaceByFileType[FileTypes.Audio] += file.FileSize;
                    break;
                case FileTypes.Image:
                    _occupiedSpaceByFileType[FileTypes.Image] += file.FileSize;
                    break;
                case FileTypes.Video:
                    _occupiedSpaceByFileType[FileTypes.Video] += file.FileSize;
                    break;
                case FileTypes.Note:
                    _occupiedSpaceByFileType[FileTypes.Note] += file.FileSize;
                    break;
                default:
                    _occupiedSpaceByFileType[FileTypes.Other] += file.FileSize;
                    break;
            }
        }
        foreach (var type in _occupiedSpaceByFileType.Keys)
        {
            _occupiedSpaceByFileType[type] = Math.Round(_occupiedSpaceByFileType[type]);
        }
    }

    private void GetSizeForStorageItems()
    {
        foreach (var item in StorageItems)
        {
            switch (item.Name)
            {
                case "Documents":
                    item.Size = _occupiedSpaceByFileType[FileTypes.Document];
                    break;
                case "Books":
                    item.Size = _occupiedSpaceByFileType[FileTypes.EBook];
                    break;
                case "Music":
                    item.Size = _occupiedSpaceByFileType[FileTypes.Audio];
                    break;
                case "Pictures":
                    item.Size = _occupiedSpaceByFileType[FileTypes.Image];
                    break;
                case "Videos":
                    item.Size = _occupiedSpaceByFileType[FileTypes.Video];
                    break;
                case "Notes":
                    item.Size = _occupiedSpaceByFileType[FileTypes.Note];
                    break;
                case "Other":
                    item.Size = _occupiedSpaceByFileType[FileTypes.Other];
                    break;
            }
        }
    }

    private async Task DrawDoughnutChart()
    {
        await JS.InvokeVoidAsync(
            "renderStorageChart",
            _occupiedSpaceByFileType[FileTypes.Document],
            _occupiedSpaceByFileType[FileTypes.EBook],
            _occupiedSpaceByFileType[FileTypes.Audio],
            _occupiedSpaceByFileType[FileTypes.Image],
            _occupiedSpaceByFileType[FileTypes.Video],
            _occupiedSpaceByFileType[FileTypes.Note],
            _occupiedSpaceByFileType[FileTypes.Other]
            );
    }

    private void CreateNewFileTypeDictionary()
    {
        _occupiedSpaceByFileType = new()
        {
            {FileTypes.Document, 0.0},
            {FileTypes.EBook, 0.0},
            {FileTypes.Audio, 0.0},
            {FileTypes.Image, 0.0},
            {FileTypes.Video, 0.0},
            {FileTypes.Note, 0.0},
            {FileTypes.Other, 0.0},
        };
    }

    private void SearchByPartialName()
    {
        var partialName = _parialNameToSearch?.Trim();

        if (!string.IsNullOrEmpty(partialName))
        {
            Console.WriteLine(partialName);
            Navigation.NavigateTo($"/search-results/{partialName}");
        }
        _parialNameToSearch = null;
    }
}
