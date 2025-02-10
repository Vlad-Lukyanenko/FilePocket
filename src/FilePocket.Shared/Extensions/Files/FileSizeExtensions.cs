namespace FilePocket.Shared.Extensions.Files;

public static class FileSizeExtensions
{
    public static double ConvertKilobytesToMegabytes(this long kilobytes)
        => kilobytes / 1024f;
}