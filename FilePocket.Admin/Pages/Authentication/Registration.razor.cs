using FilePocket.Admin.Models.Authentication;
using FilePocket.Admin.Requests.Contracts;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace FilePocket.Admin.Pages.Authentication;

public partial class Registration
{
    private const string NotificationStyle = "position: absolute; top: 530px; right: 10px;";
    private RegistrationModel _registrationModel = new();

    [Inject] NavigationManager NavigationManager { get; set; } = default!;
    [Inject] IAuthentictionRequests AuthRequest { get; set; } = default!;
    [Inject] NotificationService NotificationService { get; set; } = default!;

    private async Task OnSubmit(RegistrationModel model)
    {
        model.UserName = model.Email;
        model.Roles = new List<string>() { "User" };
        var isRegistered = await AuthRequest.RegisterUserAsync(model);

        if (isRegistered)
        {
            NavigationManager.NavigateTo("/login");
            NotificationService.Notify(
                new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Registered succesfully!",
                    Duration = 3000,
                    Style = NotificationStyle
                });
        }
        else
        {
            NotificationService.Notify(
                new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Something went wrong!",
                    Duration = 3000,
                    Style = NotificationStyle
                });
        }
    }
}
