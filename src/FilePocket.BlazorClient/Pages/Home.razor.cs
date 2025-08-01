﻿using FilePocket.BlazorClient.Features.SharedFiles.Models;
using FilePocket.BlazorClient.Features.SharedFiles.Requests;
using FilePocket.BlazorClient.Features.Users.Requests;
using FilePocket.BlazorClient.Services.Files.Models;
using FilePocket.BlazorClient.Services.Files.Requests;
using FilePocket.BlazorClient.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.ObjectModel;

namespace FilePocket.BlazorClient.Pages;

public partial class Home : ComponentBase
{
    private List<SharedFileView>? _sharedFiles;
    private ObservableCollection<FileInfoModel> _files = new();

    [Inject] private IFileRequests FileRequests { get; set; } = default!;
    [Inject] private ISharedFilesRequests SharedFilesRequests { get; set; } = default!;
    [Inject] AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] IUserRequests UserRequests { get; set; } = default!;

    // protected override async Task OnInitializedAsync()
    // {
    //     var authState = await AuthStateProvider.GetAuthenticationStateAsync();
    //     var userName = authState.User.Identity?.Name!;
    //     var user = await UserRequests.GetByUserNameAsync(userName)!;

    //     var files = await FileRequests.GetRecentFilesAsync();

    //     if (files is not null && files.Any())
    //     {
    //         if (user.Profile?.IconId != null && user.Profile.IconId != Guid.Empty)
    //         {
    //             _files = new ObservableCollection<FileInfoModel>(files.Where(f => f.Id != user.Profile!.IconId));
    //         }
    //         else
    //         {
    //             _files = new ObservableCollection<FileInfoModel>(files);
    //         }               
    //     }

    //     _sharedFiles = await SharedFilesRequests.GetLatestAsync();
    // }
    
    protected override async Task OnInitializedAsync()
{
    try
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var userName = authState.User.Identity?.Name;
        
        if (string.IsNullOrEmpty(userName))
        {
            return;
        }

        var user = await UserRequests.GetByUserNameAsync(userName);

        if (user == null)
        {
            return;
        }

        if (user.Profile != null && user.Profile.IconId != null && user.Profile.IconId != Guid.Empty)
        { 
            var files = await FileRequests.GetRecentFilesAsync();
            if (files != null && files.Any())
            {
                _files = new ObservableCollection<FileInfoModel>(files.Where(f => f.Id != user.Profile.IconId));
            }
        }
        else
        {
            var files = await FileRequests.GetRecentFilesAsync();

            if (files != null && files.Any())
            {
                _files = new ObservableCollection<FileInfoModel>(files);
            }
        }

        _sharedFiles = await SharedFilesRequests.GetLatestAsync();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Ошибка при инициализации: {ex.Message}");
    }
}


    public static FileTypes ParseEnum(string value)
    {
        return (FileTypes)Enum.Parse(typeof(FileTypes), value, true);
    }
}
