namespace FilePocket.Client.Services.Helpers
{
    public static class Tools
    {
        public static string FormatFileSize(double fileSizeInKilobytes)
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
