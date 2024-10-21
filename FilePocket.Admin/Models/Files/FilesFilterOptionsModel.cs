using Newtonsoft.Json;

namespace FilePocket.Admin.Models.Files
{
    public class FilesFilterOptionsModel : PaginationModel
    {
        private string _nameContains = string.Empty;

        public Guid UserId { get; set; }
        public Guid? StorageId { get; set; }
        public string StorageName { get; set; } = string.Empty;
        public DateTime? AfterDate { get; set; }
        public DateTime? BeforeDate { get; set; }
        public string OriginalNameContains
        {
            get => _nameContains;
            set => _nameContains = value!.ToLower();
        }
        public string FileType { get; set; } = string.Empty;
    }
}
