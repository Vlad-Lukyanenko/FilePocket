using FilePocket.BlazorClient.Features.Bookmarks.Models;
using FilePocket.BlazorClient.Features.Bookmarks.Requests;
using FilePocket.BlazorClient.Features.Users.Models;
using FilePocket.BlazorClient.Features.Users.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FilePocket.BlazorClient.Pages.Bookmarks;

public partial class CreateBookmark
{
    private string _bookmarkTitle = string.Empty;
    private string _bookmarkUrl = string.Empty;
    private bool _validTitle = true;
    private bool _validUrl = true;
    private LoggedInUserModel? _user;
    private string _userName = string.Empty;

    [Parameter] public Guid PocketId { get; set; }
    [Parameter] public Guid? FolderId { get; set; } = null;

    [Inject] IUserRequests UserRequests { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] private IBookmarkRequests BookmarkRequests { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        _userName = user.Identity?.Name!;
        _user = await UserRequests.GetByUserNameAsync(_userName);

        if (_user == null)
            return;
    }

    private async Task CreateBookmarkAsync()
    {
        if (string.IsNullOrEmpty(_bookmarkTitle))
        {
            _validTitle = false;
        }

        if (string.IsNullOrEmpty(_bookmarkUrl))
        {
            _validUrl = false;
        }

        var model = new CreateBookmarkModel()
        {
            PocketId = PocketId,
            FolderId = FolderId,
            Title = _bookmarkTitle,
            Url = _bookmarkUrl
        };

        var result = await BookmarkRequests.CreateAsync(model);

        if (result)
        {
            var redirectionUri = Navigation.Uri.Replace("/new", string.Empty);
            Navigation.NavigateTo(redirectionUri);
        }
    }

    private void TitleChanged()
    {
        _validTitle = !string.IsNullOrEmpty(_bookmarkTitle);
    }

    private void UrlChanged()
    {
        _validUrl = !string.IsNullOrEmpty(_bookmarkUrl);
    }
}
