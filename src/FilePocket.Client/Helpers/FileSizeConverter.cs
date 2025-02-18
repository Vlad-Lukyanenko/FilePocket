namespace FilePocket.Client.Helpers;

public static class FileSizeConverter
{
    public static string ConvertBytes(double? bytes)
    {
        if (bytes is null)
        {
            return "0";
        }

        const double kb = 1024;
        const double mb = kb * 1024;
        const double gb = mb * 1024;

        return bytes switch
        {
            < kb => $"{bytes} B",
            < mb => $"{bytes / kb:0.##} KB",
            < gb => $"{bytes / mb:0.##} MB",
            _ => $"{bytes / gb:0.##} GB"
        };
    }
        
    public static string ConvertMegabytes(double? megabytes)
    {
        if (megabytes is null)
        {
            return "0";
        }

        const double kb = 1024;
        const double gb = 1024;

        return megabytes switch
        {
            < 1 => $"{megabytes * kb:0.##} KB", // If megabytes is less than 1, display as KB
            < gb => $"{megabytes:0.##} MB", // If megabytes is less than 1 GB, display as MB
            _ => $"{megabytes / gb:0.##} GB", // Otherwise, display as GB
        };
    }
}