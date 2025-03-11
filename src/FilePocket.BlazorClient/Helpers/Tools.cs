using FilePocket.BlazorClient.Features.Files.Models;

namespace FilePocket.BlazorClient.Helpers
{
    public static class Tools
    {
        public const int MaxDescriptionLength = 500;

        public static string TruncateFileName(string fullName, int maxLen = 15)
        {
            var ext = Path.GetExtension(fullName);
            var name = Path.GetFileNameWithoutExtension(fullName);

            if (name.Length > maxLen)
            {
                name = name.Substring(0, maxLen) + "...";
            }

            return name + ext;
        }

        public static string TruncateString(string input, int maxLen = 15)
        {
            if (input.Length > maxLen)
            {
                input = input.Substring(0, maxLen) + "...";
            }

            return input;
        }

        public static string GetIconName(FileTypes? fileType)
        {
            return fileType switch
            {
                FileTypes.Image => "image.png",
                FileTypes.EBook => "book.png",
                FileTypes.Document => "document.png",
                FileTypes.Video => "video.png",
                FileTypes.Audio => "audio.png",
                FileTypes.Archive => "archive.png",
                FileTypes.Code => "script.png",
                FileTypes.Font => "font.png",
                FileTypes.Other => "other.png",
                FileTypes.Text => "txt-file.png",
                _ => "other.png",
            };
        }

        public static string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                // Text
                ".txt" => "text/plain",
                ".csv" => "text/csv",
                ".log" => "text/plain",
                ".xml" => "application/xml",
                ".html" => "text/html",
                ".htm" => "text/html",
                ".json" => "application/json",

                // Images
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".svg" => "image/svg+xml",
                ".ico" => "image/vnd.microsoft.icon",
                ".tiff" => "image/tiff",
                ".webp" => "image/webp",

                // Audio
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".ogg" => "audio/ogg",
                ".m4a" => "audio/mp4",
                ".flac" => "audio/flac",

                // Video
                ".mp4" => "video/mp4",
                ".avi" => "video/x-msvideo",
                ".mov" => "video/quicktime",
                ".wmv" => "video/x-ms-wmv",
                ".flv" => "video/x-flv",
                ".mkv" => "video/x-matroska",
                ".webm" => "video/webm",

                // Documents
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",

                // eBooks
                ".epub" => "application/epub+zip",
                ".mobi" => "application/x-mobipocket-ebook",
                ".fb2" => "application/x-fictionbook+xml",

                // Archives
                ".zip" => "application/zip",
                ".rar" => "application/vnd.rar",
                ".7z" => "application/x-7z-compressed",
                ".tar" => "application/x-tar",
                ".gz" => "application/gzip",

                // Code
                ".js" => "application/javascript",
                ".css" => "text/css",
                ".java" => "text/x-java-source",
                ".c" => "text/x-c",
                ".cpp" => "text/x-c",
                ".py" => "text/x-python",
                ".php" => "application/x-httpd-php",
                ".sh" => "application/x-sh",
                ".bat" => "application/x-msdos-program",

                // Fonts
                ".ttf" => "font/ttf",
                ".otf" => "font/otf",
                ".woff" => "font/woff",
                ".woff2" => "font/woff2",

                // Fallback
                _ => "application/octet-stream", // Default MIME type
            };
        }
    }
}
