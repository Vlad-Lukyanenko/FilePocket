using FilePocket.Admin.Models.Storage;
using Microsoft.AspNetCore.Components;

namespace FilePocket.Admin.Components.Dialogs
{
    public partial class StorageDetailsDialog :ComponentBase
    {
        [Parameter] public StorageModel Storage { get; set; } = default!;
        [Parameter] public Func<StorageModel, Task> OnSubmit { get; set; } = default!;

        private string FormatFileSize(double fileSizeInKilobytes)
        {
            double fileSizeInBytes = fileSizeInKilobytes * 1024;
            if (fileSizeInBytes >= 1024 * 1024 * 1024)
                return $"{(fileSizeInBytes / 1024.0 / 1024.0 / 1024.0):F2} GB";
            else if (fileSizeInBytes >= 1024 * 1024)
                return $"{(fileSizeInBytes / 1024.0 / 1024.0):F2} MB";
            else if (fileSizeInBytes >= 1024)
                return $"{(fileSizeInBytes / 1024.0):F2} KB";
            else
                return $"{Math.Round(fileSizeInBytes)} B";
        }
    }
}