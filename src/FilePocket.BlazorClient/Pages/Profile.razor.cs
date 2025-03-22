using FilePocket.BlazorClient.Features.Profiles.Models;
using FilePocket.BlazorClient.Features.Profiles.Requests;
using FilePocket.BlazorClient.Features.Users.Models;
using FilePocket.BlazorClient.Features.Users.Requests;
using FilePocket.BlazorClient.Services.Files.Models;
using FilePocket.BlazorClient.Services.Files.Requests;
using FilePocket.BlazorClient.Services.Pockets.Requests;
using FilePocket.BlazorClient.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;

namespace FilePocket.BlazorClient.Pages;

public partial class Profile : ComponentBase
{
    private ProfileModel _profile = new();
    private bool _isLoading = true;
    private string _alertMessage = "";
    private string _alertStyle = "display: none;";
    private ObservableCollection<FileUploadError> _fileUploadErrors = [];
    private Guid _defaultPocketId;
    private FileModel _avatar = new();
    private IBrowserFile? _file;

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] private IUserRequests UserRequests { get; set; } = default!;
    [Inject] private IProfileRequests ProfileRequests { get; set; } = default!;
    [Inject] private IPocketRequests PocketRequests { get; set; } = default!;
    [Inject] private IFileRequests FileRequests { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userAuth = authState.User;
        var userStringId = userAuth.FindFirst(c => c.Type == "uid")?.Value;
        var userId = new Guid(userStringId!);

        if (userId != Guid.Empty)
        {
            _profile = await ProfileRequests.GetByUserIdAsync(userId);
            _defaultPocketId = (await PocketRequests.GetDefaultAsync()).Id;

            if (_profile.IconId is not null && _profile.IconId != Guid.Empty)
            {
                _avatar = await FileRequests.GetImageThumbnailAsync((Guid)_profile.IconId, 500);
            }
        }

        _isLoading = false;
    }

    private async Task SaveChangesAsync(MouseEventArgs e)
    {
        if (string.IsNullOrEmpty(_profile.FirstName) || string.IsNullOrEmpty(_profile.LastName))
        {
            return;
        }

        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        var request = new UpdateUserRequest
        {
            UserName = authState.User.Identity?.Name!,
            FirstName = _profile.FirstName,
            LastName = _profile.LastName
        };

        await UserRequests.UpdateUserAsync(request);

        if (_file is not null)
        {
            await SaveAvatarToDbAsync();
        }

        var profileToUpdate = new UpdateProfileModel
        {
            Id = _profile.Id,
            FirstName = _profile.FirstName,
            LastName = _profile.LastName,
            IconId = _profile.IconId
        };

        var isUpdated = await ProfileRequests.UpdateAsync(profileToUpdate);

        if (isUpdated)
        {
            await TriggerNotification();
        }
    }

    private string GetGoBackUrl()
    {
        return string.Empty;
    }

    private async Task TriggerNotification()
    {
        await ShowAlert("Updates successfully saved");
        await Task.Delay(3000);
        await CloseAlert();
    }

    private async Task ShowAlert(string message)
    {
        _alertMessage = message;
        _alertStyle = "display: block;";

        await InvokeAsync(StateHasChanged);
    }

    private async Task CloseAlert()
    {
        _alertMessage = "";
        _alertStyle = "display: none;";

        await InvokeAsync(StateHasChanged);
    }

    private async Task UploadFilesAsync(InputFileChangeEventArgs e)
    {
        _file = e.File;

        if (_file is null)
        {
            Console.WriteLine("No files to process.");
            return;
        }

        _avatar.FileByteArray = await GetFileBytesAsync(_file);

        StateHasChanged();

        await ResetFileInput();
    }

    private async Task SaveAvatarToDbAsync()
    {
        try
        {
            var maxFileSize = 1024 * 1024 * 1024;

            using (var fileContent = new StreamContent(_file!.OpenReadStream(maxFileSize)))
            {
                using (var content = new MultipartFormDataContent())
                {
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(GetContentType(_file.ContentType));

                    var folderId = string.Empty;
                    var pocketId = _defaultPocketId.ToString();

                    content.Add(content: fileContent, name: "file", fileName: _file.Name);
                    content.Add(content: new StringContent(pocketId), "PocketId");
                    content.Add(content: new StringContent(folderId), "FolderId");

                    var uploadedFile = await FileRequests.UploadFileAsync(content);

                    if (_profile.IconId is not null && _profile.IconId != Guid.Empty)
                    {
                        await FileRequests.DeleteFile((Guid)_profile.IconId);
                    }

                    if (uploadedFile is not null)
                    {
                        _profile.IconId = uploadedFile.Id;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing file {_file!.Name}: {ex.Message}");
            await InvokeAsync(() =>
            {
                if (ex is RequestHandlingErrorException exception)
                {
                    var error = exception.Error;
                    _fileUploadErrors.Add(new FileUploadError { OriginalName = _file.Name, ErrorMessage = error.Message! });
                }
                else
                {
                    _fileUploadErrors.Add(new FileUploadError { OriginalName = _file.Name, ErrorMessage = ex.Message });
                }

                StateHasChanged();
            });
        }
    }

    private static string GetContentType(string fileContentType)
    {
        return string.IsNullOrWhiteSpace(fileContentType) ? "application/octet-stream" : fileContentType;
    }

    private async Task ResetFileInput()
    {
        await JS.InvokeVoidAsync("resetFileInput", "fileInput");
    }

    private void RemoveError(FileUploadError error)
    {
        _fileUploadErrors.Remove(error);
    }

    private string GetProfileIcon()
    {
        if (_avatar.FileByteArray is null)
        {
            return "../assets/img/man.png";
        }

        return $"data:image/jpeg;base64,{Convert.ToBase64String(_avatar.FileByteArray!)}";
    }

    private static async Task<byte[]> GetFileBytesAsync(IBrowserFile file)
    {
        using var memoryStream = new MemoryStream();

        await file!.OpenReadStream(maxAllowedSize: 1024 * 1024 * 1024).CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }
}
