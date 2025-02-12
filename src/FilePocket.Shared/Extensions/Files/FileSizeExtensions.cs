namespace FilePocket.Shared.Extensions.Files;

public static class FileSizeExtensions
{
    public static double ToMegabytes(this long bytes)
        => bytes / 1024f / 1024f;

    public static double ToBytes(this double megabytes)
        => megabytes * 1024 * 1024;
}