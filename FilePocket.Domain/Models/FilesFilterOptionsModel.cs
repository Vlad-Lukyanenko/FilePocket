namespace FilePocket.Domain.Models
{
    public record FilesFilterOptionsModel(
        Guid UserId,
        Guid? StorageId,
        string StorageName,
        DateTime? AfterDate,
        DateTime? BeforeDate,
        string OriginalNameContains,
        string FileType,
        int PageSize,
        int PageNumber);
}
