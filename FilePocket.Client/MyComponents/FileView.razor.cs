using FilePocket.Client.Services.Files.Models;
using FilePocket.Client.Services.Files.Requests;
using FilePocket.Client.Services.Folders.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FilePocket.Client.MyComponents
{
    public partial class FileView
    {
        [Parameter]
        public string PocketId { get; set; } = string.Empty;

        [Parameter]
        public string FolderId { get; set; } = string.Empty;

        [Parameter]
        public string FileId { get; set; } = string.Empty;

        [Inject]
        private IFileRequests FileRequests { get; set; } = default!;

        [Inject]
        private IFolderRequests FolderRequests { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private Guid _pocketId;
        private Guid _fileId;

        private FileModel? _file;
        private string _imageContent = string.Empty;
        private string _goBackUrl = string.Empty;

        private void InitGoBackUrl()
        {
            _goBackUrl = string.IsNullOrWhiteSpace(FolderId)
               ? $"/pockets/{PocketId}/files"
               : $"/pockets/{PocketId}/folders/{FolderId}/files";
        }

        protected override async Task OnInitializedAsync()
        {
            _pocketId = Guid.Parse(PocketId);
            _fileId = Guid.Parse(FileId);

            _file = await FileRequests.GetFileInfoAsync(_pocketId, _fileId);

            if (_file.FileType == "Image")
            {
                _file = await FileRequests.GetImageThumbnailAsync(_pocketId, _fileId, 500);

                string base64 = Convert.ToBase64String(new ReadOnlySpan<byte>(_file.FileByteArray!));
                var mimeType = GetMimeType(_file.OriginalName!);
                _imageContent = $"data:{mimeType};base64,{base64}";
            }

            InitGoBackUrl();
        }

        private async void DownloadFile()
        {
            var file = await FileRequests.GetFileAsync(_pocketId, _fileId);
            var base64 = Convert.ToBase64String(file!.FileByteArray!);
            var mimeType = GetMimeType(file.OriginalName!);
            await JSRuntime.InvokeVoidAsync("saveFile", file.OriginalName, mimeType, base64);
        }

        private string GetMimeType(string fileName)
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
