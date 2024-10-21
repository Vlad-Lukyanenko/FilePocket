using FilePocket.Admin.AuthFeatures;
using FilePocket.Admin.Models.Authentication;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace FilePocket.Admin.Pages.Authentication;

public partial class Login
{
    private const string NotificationStyle = "position: absolute; top: 530px; right: 10px;";

    [Inject] AuthStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] NavigationManager NavigationManager { get; set; } = default!;
    [Inject] NotificationService NotificationService { get; set; } = default!;

    private async Task LoginAsync(LoginArgs args)
    {
        var loginModel = new LoginModel { Email = args.Username, Password = args.Password };

        var isLogged = await AuthStateProvider.LoginAsync(loginModel);

        if (isLogged)
        {
            NavigationManager.NavigateTo("/", true);
        }
        else
        {
            NotificationService.Notify(
                new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Email or password is incorrect.",
                    Duration = 3000,
                    Style = NotificationStyle
                });
        }        
    }
}
