namespace FilePocket.Admin.Models.Files
{
    public class FilteredFilesModel
    {
        public List<FileModel>? Files { get; set; } = [];
        public int PagesCount { get; set; } = default;
        public int ItemsCount { get; set; } = default;
    }
}
