using Microsoft.AspNetCore.Components;

namespace FilePocket.Admin.Components.Dialogs
{
    public partial class ThumbnailDialog :ComponentBase
    {
        [Parameter] public string ImageUrl { get; set; }
        [Parameter] public string FileName { get; set; }
        [Parameter] public DateTime DateCreated { get; set; }
        [Parameter] public double? FileSize { get; set; }
    }
}