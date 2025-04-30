using FilePocket.Domain;

namespace FilePocket.Application
{
    public static class Tools
    {
        public static FileTypes DefineFileType(string fileExtension)
        {
            FileTypes fileType;

            switch (fileExtension.ToLower())
            {
                case ".txt":
                case ".csv":
                case ".log":
                case ".xml":
                case ".html":
                case ".htm":
                case ".json":
                    fileType = FileTypes.Text;
                    break;

                case ".pdf":
                case ".doc":
                case ".docx":
                case ".xls":
                case ".xlsx":
                case ".ppt":
                case ".pptx":
                case ".rtf":
                    fileType = FileTypes.Document;
                    break;

                case ".png":
                case ".jpeg":
                case ".jpg":
                case ".gif":
                case ".bmp":
                case ".svg":
                case ".ico":
                case ".tiff":
                case ".webp":
                    fileType = FileTypes.Image;
                    break;

                case ".mp3":
                case ".wav":
                case ".ogg":
                case ".m4a":
                case ".flac":
                case ".vma":
                    fileType = FileTypes.Audio;
                    break;

                case ".mp4":
                case ".avi":
                case ".mov":
                case ".wmv":
                case ".flv":
                case ".mkv":
                case ".webm":
                    fileType = FileTypes.Video;
                    break;

                case ".epub":
                case ".mobi":
                case ".fb2":
                    fileType = FileTypes.EBook;
                    break;

                case ".zip":
                case ".rar":
                case ".7z":
                case ".tar":
                case ".gz":
                    fileType = FileTypes.Archive;
                    break;

                case ".js":
                case ".css":
                case ".java":
                case ".c":
                case ".cpp":
                case ".py":
                case ".php":
                case ".sh":
                case ".bat":
                    fileType = FileTypes.Code;
                    break;

                case ".ttf":
                case ".otf":
                case ".woff":
                case ".woff2":
                    fileType = FileTypes.Font;
                    break;

                case ".fpn":
                    fileType = FileTypes.Note;
                    break;

                default:
                    fileType = FileTypes.Other;
                    break;
            }

            return fileType;
        }
    }
}
